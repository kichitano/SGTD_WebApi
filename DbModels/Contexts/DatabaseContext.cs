using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.DbModels.Enums;
using SGTD_WebApi.Models.LogSystem;

namespace SGTD_WebApi.DbModels.Contexts;

/// <summary>
/// Contexto de base de datos principal del sistema SGTD que maneja todas las entidades y auditoría
/// </summary>
public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ActionTypeEnum? _currentAction;

    /// <summary>
    /// Inicializa una nueva instancia del contexto de base de datos
    /// </summary>
    /// <param name="options">Opciones de configuración para el contexto</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <param name="httpContextAccessor">Acceso al contexto HTTP para auditoría</param>
    public DatabaseContext(DbContextOptions<DatabaseContext> options, 
        IConfiguration configuration, 
        IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Area> Areas { get; set; }
    public DbSet<AreaDependency> AreaDependencies { get; set; }
    public DbSet<Authenticator> Authenticators { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<DocumentaryProcedure> DocumentaryProcedures { get; set; }
    public DbSet<DocumentaryProcedureStep> DocumentaryProcedureSteps { get; set; }
    public DbSet<DocumentaryProcedureStepDocument> DocumentaryProcedureStepDocuments { get; set; }
    public DbSet<DocumentaryProcessInstance> DocumentaryProcessInstances { get; set; }
    public DbSet<DocumentaryProcessStepInstance> DocumentaryProcessStepInstances { get; set; }
    public DbSet<DocumentaryProcessDocument> DocumentaryProcessDocuments { get; set; }
    public DbSet<DocumentaryProcessNotification> DocumentaryProcessNotifications { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<PositionDependency> PositionsDependency { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }
    public DbSet<UserFileShare> UserFileShares { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleComponentPermission> RoleComponentPermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<UserDigitalSignature> UserDigitalSignatures { get; set; }
    public DbSet<LogSystem> LogSystems { get; set; }

    /// <summary>
    /// Guarda los cambios en la base de datos de forma asíncrona con auditoría automática
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>El número de entidades afectadas</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsyncWithAudit(cancellationToken);
    }

    private async Task<int> SaveChangesAsyncWithAudit(CancellationToken cancellationToken = default)
    {
        var auditEntries = OnBeforeSaveChanges();
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries);
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is LogSystem || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                EntityName = entry.Entity.GetType().Name,
                Action = _currentAction ?? GetActionTypeEnum(entry.State)
            };

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }

            if (auditEntry.NewValues.Count > 0 || auditEntry.OldValues.Count > 0)
            {
                auditEntries.Add(auditEntry);
            }
        }

        return auditEntries;
    }

    private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
    {
        if (auditEntries.Count == 0)
            return;

        var userId = GetCurrentUserId();

        foreach (var auditEntry in auditEntries)
        {
            LogSystems.Add(new LogSystem
            {
                EntityName = auditEntry.EntityName,
                Action = auditEntry.Action,
                PreviousValue = auditEntry.OldValues.Count == 0 ? null : JsonConvert.SerializeObject(auditEntry.OldValues),
                NewValue = auditEntry.NewValues.Count == 0 ? null : JsonConvert.SerializeObject(auditEntry.NewValues),
                Timestamp = DateTime.UtcNow,
                UserId = userId,
            });
        }

        await base.SaveChangesAsync();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId) ? userId : Guid.Empty;
    }

    private ActionTypeEnum GetActionTypeEnum(EntityState state)
    {
        switch (state)
        {
            case EntityState.Added:
                return ActionTypeEnum.Create;
            case EntityState.Modified:
                return ActionTypeEnum.Update;
            case EntityState.Deleted:
                return ActionTypeEnum.Delete;
            default:
                return ActionTypeEnum.Read;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Base).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(Base.IsDeleted));
                var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

        void ConfigureOneToManyRelationship<TEntity, TRelated>(
            ModelBuilder mBuilder,
            Expression<Func<TEntity, TRelated>> navigationPropertyExpression,
            Expression<Func<TEntity, object>> foreignKeyExpression)
            where TEntity : class
            where TRelated : class
        {
            mBuilder.Entity<TEntity>()
                .HasOne(navigationPropertyExpression!)
                .WithMany()
                .HasForeignKey(foreignKeyExpression!)
                .OnDelete(DeleteBehavior.NoAction);
        }
        
        ConfigureOneToManyRelationship<AreaDependency, Area>(modelBuilder, ad => ad.ParentArea, ad => ad.ParentAreaId);
        ConfigureOneToManyRelationship<AreaDependency, Area>(modelBuilder, ad => ad.ChildArea, ad => ad.ChildAreaId);

        ConfigureOneToManyRelationship<DocumentaryProcedure, Area>(modelBuilder, dp => dp.Area, dp => dp.AreaId);

        ConfigureOneToManyRelationship<DocumentaryProcedureStep, Area>(modelBuilder, dps => dps.Area, dps => dps.AreaId);
        ConfigureOneToManyRelationship<DocumentaryProcedureStep, Position>(modelBuilder, dps => dps.Position, dps => dps.PositionId);
        ConfigureOneToManyRelationship<DocumentaryProcedureStep, DocumentaryProcedure>(modelBuilder, dps => dps.DocumentaryProcedure, dps => dps.DocumentaryProcedureId);

        ConfigureOneToManyRelationship<DocumentaryProcedureStepDocument, DocumentaryProcedureStep>(modelBuilder, dpsd => dpsd.DocumentaryProcedureStep, dpsd => dpsd.DocumentaryProcedureStepId);
        ConfigureOneToManyRelationship<DocumentaryProcedureStepDocument, DocumentType>(modelBuilder, dpsd => dpsd.DocumentType, dpsd => dpsd.DocumentTypeId);

        modelBuilder.Entity<DocumentaryProcessInstance>()
            .HasOne(dpi => dpi.DocumentaryProcedure)
            .WithMany()
            .HasForeignKey(dpi => dpi.DocumentaryProcedureId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessInstance>()
            .HasOne(dpi => dpi.RequestedByUser)
            .WithMany()
            .HasForeignKey(dpi => dpi.RequestedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessStepInstance>()
            .HasOne(dpsi => dpsi.DocumentaryProcessInstance)
            .WithMany(dpi => dpi.StepInstances)
            .HasForeignKey(dpsi => dpsi.DocumentaryProcessInstanceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessStepInstance>()
            .HasOne(dpsi => dpsi.DocumentaryProcedureStep)
            .WithMany()
            .HasForeignKey(dpsi => dpsi.DocumentaryProcedureStepId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessStepInstance>()
            .HasOne(dpsi => dpsi.AssignedToUser)
            .WithMany()
            .HasForeignKey(dpsi => dpsi.AssignedToUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessDocument>()
            .HasOne(dpd => dpd.DocumentaryProcessInstance)
            .WithMany(dpi => dpi.Documents)
            .HasForeignKey(dpd => dpd.DocumentaryProcessInstanceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessDocument>()
            .HasOne(dpd => dpd.DocumentaryProcessStepInstance)
            .WithMany(dpsi => dpsi.Documents)
            .HasForeignKey(dpd => dpd.DocumentaryProcessStepInstanceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessDocument>()
            .HasOne(dpd => dpd.DocumentType)
            .WithMany()
            .HasForeignKey(dpd => dpd.DocumentTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessDocument>()
            .HasOne(dpd => dpd.UploadedByUser)
            .WithMany()
            .HasForeignKey(dpd => dpd.UploadedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessNotification>()
            .HasOne(dpn => dpn.DocumentaryProcessInstance)
            .WithMany()
            .HasForeignKey(dpn => dpn.DocumentaryProcessInstanceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocumentaryProcessNotification>()
            .HasOne(dpn => dpn.User)
            .WithMany()
            .HasForeignKey(dpn => dpn.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        ConfigureOneToManyRelationship<PositionDependency, Position>(modelBuilder, pd => pd.ParentPosition, pd => pd.ParentPositionId);
        ConfigureOneToManyRelationship<PositionDependency, Position>(modelBuilder, pd => pd.ChildPosition, pd => pd.ChildPositionId);

        ConfigureOneToManyRelationship<UserRole, User>(modelBuilder, ur => ur.User, ur => ur.UserId);
        ConfigureOneToManyRelationship<UserRole, Role>(modelBuilder, ur => ur.Role, ur => ur.RoleId);

        ConfigureOneToManyRelationship<RoleComponentPermission, Role>(modelBuilder, rcp => rcp.Role, rcp => rcp.RoleId);
        ConfigureOneToManyRelationship<RoleComponentPermission, Component>(modelBuilder, rcp => rcp.Component, rcp => rcp.ComponentId);
        ConfigureOneToManyRelationship<RoleComponentPermission, Permission>(modelBuilder, rcp => rcp.Permission, rcp => rcp.PermissionId);

        ConfigureOneToManyRelationship<User, Person>(modelBuilder, u => u.Person, u => u.PersonId);

        ConfigureOneToManyRelationship<UserFile, User>(modelBuilder, u => u.User, u => u.UserId);
        
        ConfigureOneToManyRelationship<UserFileShare, UserFile>(modelBuilder, ufs => ufs.UserFile, ufs => ufs.UserFileId);
        ConfigureOneToManyRelationship<UserFileShare, User>(modelBuilder, ufs => ufs.SharedWithUser, ufs => ufs.SharedWithUserId);
        ConfigureOneToManyRelationship<UserFileShare, User>(modelBuilder, ufs => ufs.SharedByUser, ufs => ufs.SharedByUserId);
    }
}