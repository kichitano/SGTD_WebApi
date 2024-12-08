using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.DbModel.Enums;
using SGTD_WebApi.Models.LogSystem;

namespace SGTD_WebApi.DbModel.Context;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ActionTypeEnum? _currentAction;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, 
        IConfiguration configuration, 
        IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;

        try
        {
            var dbCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (dbCreator != null)
            {
                if (!dbCreator.CanConnect())
                    dbCreator.Create();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) return;
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);
    }

    public DbSet<Area> Areas { get; set; }
    public DbSet<AreaDependency> AreaDependencies { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<DocumentaryProcedure> DocumentaryProcedures { get; set; }
    public DbSet<DocumentaryProcedureStep> DocumentaryProcedureSteps { get; set; }
    public DbSet<DocumentaryProcedureStepDocument> DocumentaryProcedureStepDocuments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<PositionDependency> PositionsDependency { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleComponentPermission> RoleComponentPermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<LogSystem> LogSystems { get; set; }

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

        ConfigureOneToManyRelationship<PositionDependency, Position>(modelBuilder, pd => pd.ParentPosition, pd => pd.ParentPositionId);
        ConfigureOneToManyRelationship<PositionDependency, Position>(modelBuilder, pd => pd.ChildPosition, pd => pd.ChildPositionId);

        ConfigureOneToManyRelationship<UserRole, User>(modelBuilder, ur => ur.User, ur => ur.UserId);
        ConfigureOneToManyRelationship<UserRole, Role>(modelBuilder, ur => ur.Role, ur => ur.RoleId);

        ConfigureOneToManyRelationship<RoleComponentPermission, Role>(modelBuilder, rcp => rcp.Role, rcp => rcp.RoleId);
        ConfigureOneToManyRelationship<RoleComponentPermission, Component>(modelBuilder, rcp => rcp.Component, rcp => rcp.ComponentId);
        ConfigureOneToManyRelationship<RoleComponentPermission, Permission>(modelBuilder, rcp => rcp.Permission, rcp => rcp.PermissionId);

        ConfigureOneToManyRelationship<User, Person>(modelBuilder, u => u.Person, u => u.PersonId);

        ConfigureOneToManyRelationship<UserFile, User>(modelBuilder, u => u.User, u => u.UserId);
    }
}