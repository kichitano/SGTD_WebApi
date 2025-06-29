-- Script unificado de población de datos para SGTD
-- Este script combina datos completos con escenarios específicos para pruebas de autoridad máxima
-- Ejecutar después de aplicar las migraciones

-- ============================================
-- LIMPIAR DATOS EXISTENTES (excepto el primer usuario)
-- ============================================
DELETE FROM "UserRoles" WHERE "UserId" > 1;
DELETE FROM "Users" WHERE "Id" > 1;
DELETE FROM "Authenticators" WHERE "UserGuid" NOT IN (SELECT "UserGuid" FROM "Users" WHERE "Id" = 1);
DELETE FROM "Persons" WHERE "Id" > 1;
DELETE FROM "Positions" WHERE "Id" > 1;
DELETE FROM "Areas" WHERE "Id" > 1;
DELETE FROM "DocumentTypes" WHERE "Id" > 1;
DELETE FROM "Roles" WHERE "Id" > 1;

-- Resetear secuencias
SELECT setval('"Areas_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Areas"), 1));
SELECT setval('"Persons_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Persons"), 1));
SELECT setval('"Positions_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Positions"), 1));
SELECT setval('"Users_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Users"), 1));
SELECT setval('"Roles_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Roles"), 1));
SELECT setval('"DocumentTypes_Id_seq"', COALESCE((SELECT MAX("Id") FROM "DocumentTypes"), 1));

-- ============================================
-- INSERTAR ROLES
-- ============================================
INSERT INTO "Roles" ("Name", "Description", "CreatedAt", "UpdatedAt") VALUES
('Administrador', 'Rol con acceso completo al sistema', NOW(), NOW()),
('Director', 'Director de área con máxima autoridad', NOW(), NOW()),
('Gerente', 'Rol de gerencia con permisos de supervisión', NOW(), NOW()),
('Jefe de Área', 'Jefe responsable de un área específica', NOW(), NOW()),
('Especialista', 'Especialista técnico con conocimientos específicos', NOW(), NOW()),
('Analista', 'Analista con permisos especializados', NOW(), NOW()),
('Empleado', 'Empleado regular con permisos básicos', NOW(), NOW()),
('Asistente', 'Personal de apoyo administrativo', NOW(), NOW());

-- ============================================
-- INSERTAR TIPOS DE DOCUMENTO
-- ============================================
INSERT INTO "DocumentTypes" ("Name", "IsUploadable", "CreatedAt", "UpdatedAt") VALUES
('Solicitud de Permiso', true, NOW(), NOW()),
('Informe Técnico', false, NOW(), NOW()),
('Memorandum', true, NOW(), NOW()),
('Resolución', false, NOW(), NOW()),
('Oficio', true, NOW(), NOW()),
('Carta', true, NOW(), NOW()),
('Acta', false, NOW(), NOW()),
('Constancia', false, NOW(), NOW()),
('Certificado', false, NOW(), NOW()),
('Reporte de Gastos', true, NOW(), NOW());

-- ============================================
-- INSERTAR ÁREAS
-- ============================================
INSERT INTO "Areas" ("Name", "Description", "Status", "CreatedAt", "UpdatedAt") VALUES
('Dirección General', 'Dirección ejecutiva de la organización', true, NOW(), NOW()),
('Recursos Humanos', 'Gestión del talento humano y desarrollo organizacional', true, NOW(), NOW()),
('Tecnologías de Información', 'Desarrollo y soporte tecnológico', true, NOW(), NOW()),
('Finanzas y Contabilidad', 'Gestión financiera y contable', true, NOW(), NOW()),
('Logística y Compras', 'Gestión de adquisiciones y logística', true, NOW(), NOW()),
('Legal y Cumplimiento', 'Asesoría legal y cumplimiento normativo', true, NOW(), NOW()),
('Marketing y Comunicaciones', 'Estrategias de marketing y comunicación institucional', true, NOW(), NOW()),
('Operaciones', 'Gestión de procesos operativos principales', true, NOW(), NOW()),
('Planeamiento Estratégico', 'Planificación y seguimiento estratégico institucional', true, NOW(), NOW()),
('Auditoría Interna', 'Control interno y auditoría de procesos', true, NOW(), NOW());

-- ============================================
-- INSERTAR CARGOS/POSICIONES CON JERARQUÍA DIRECTA
-- ============================================

-- ÁREA 1: Dirección General - CON autoridad máxima
INSERT INTO "Positions" ("Name", "Description", "AreaId", "DirectManagerPositionId", "CreatedAt", "UpdatedAt") VALUES
('Gerente General', 'Máxima autoridad ejecutiva de la organización', 1, NULL, NOW(), NOW());

-- ÁREA 2: Recursos Humanos - CON autoridad máxima y jerarquía
INSERT INTO "Positions" ("Name", "Description", "AreaId", "DirectManagerPositionId", "CreatedAt", "UpdatedAt") VALUES
('Director de RRHH', 'Máxima autoridad del área de recursos humanos', 2, NULL, NOW(), NOW()),
('Jefe de Selección', 'Responsable de procesos de selección', 2, 2, NOW(), NOW()),
('Especialista en Capacitación', 'Especialista en programas de capacitación', 2, 2, NOW(), NOW()),
('Especialista en Selección', 'Encargado de procesos de selección de personal', 2, 3, NOW(), NOW()),
('Asistente de RRHH', 'Asistente administrativo de recursos humanos', 2, 5, NOW(), NOW());

-- ÁREA 3: TI - CON autoridad máxima y jerarquía
INSERT INTO "Positions" ("Name", "Description", "AreaId", "DirectManagerPositionId", "CreatedAt", "UpdatedAt") VALUES
('Director de TI', 'Máxima autoridad del área de tecnología', 3, NULL, NOW(), NOW()),
('Jefe de Desarrollo', 'Responsable de desarrollo de software', 3, 7, NOW(), NOW()),
('Analista de Sistemas', 'Analista de sistemas y procesos tecnológicos', 3, 7, NOW(), NOW()),
('Desarrollador Senior', 'Desarrollador de software con experiencia avanzada', 3, 8, NOW(), NOW()),
('Desarrollador Junior', 'Desarrollador con experiencia inicial', 3, 10, NOW(), NOW());

-- ÁREA 4: Finanzas - CON autoridad máxima y jerarquía
INSERT INTO "Positions" ("Name", "Description", "AreaId", "DirectManagerPositionId", "CreatedAt", "UpdatedAt") VALUES
('Jefe de Finanzas', 'Máxima autoridad del área financiera', 4, NULL, NOW(), NOW()),
('Contador General', 'Responsable de la contabilidad general', 4, 12, NOW(), NOW()),
('Analista Financiero', 'Analista de información financiera', 4, 12, NOW(), NOW()),
('Asistente Contable', 'Asistente en labores contables', 4, 13, NOW(), NOW());

-- ÁREA 5: Logística - SIN autoridad máxima (para prueba de nuevo cargo sin jefe)
-- Esta área está vacía inicialmente para probar registro del primer cargo

-- ÁREA 6: Legal - CON autoridad máxima
INSERT INTO "Positions" ("Name", "Description", "AreaId", "DirectManagerPositionId", "CreatedAt", "UpdatedAt") VALUES
('Jefe Legal', 'Máxima autoridad del área legal', 6, NULL, NOW(), NOW()),
('Asistente Legal', 'Asistente en temas legales y normativos', 6, 16, NOW(), NOW());

-- ÁREA 7: Marketing - SIN autoridad máxima (para prueba)
-- Esta área está vacía inicialmente

-- ÁREA 8: Operaciones - CON autoridad máxima
INSERT INTO "Positions" ("Name", "Description", "AreaId", "DirectManagerPositionId", "CreatedAt", "UpdatedAt") VALUES
('Jefe de Operaciones', 'Máxima autoridad del área operativa', 8, NULL, NOW(), NOW()),
('Supervisor de Operaciones', 'Supervisor de procesos operativos', 8, 18, NOW(), NOW()),
('Operario', 'Personal operativo de procesos', 8, 19, NOW(), NOW());

-- ÁREA 9: Planeamiento - CON autoridad máxima
INSERT INTO "Positions" ("Name", "Description", "AreaId", "DirectManagerPositionId", "CreatedAt", "UpdatedAt") VALUES
('Jefe de Planeamiento', 'Máxima autoridad de planeamiento estratégico', 9, NULL, NOW(), NOW()),
('Analista de Planeamiento', 'Analista en planificación estratégica', 9, 21, NOW(), NOW());

-- ÁREA 10: Auditoría - CON autoridad máxima
INSERT INTO "Positions" ("Name", "Description", "AreaId", "DirectManagerPositionId", "CreatedAt", "UpdatedAt") VALUES
('Auditor Jefe', 'Máxima autoridad del área de auditoría', 10, NULL, NOW(), NOW()),
('Auditor Junior', 'Auditor con experiencia inicial', 10, 23, NOW(), NOW());

-- ============================================
-- INSERTAR PERSONAS
-- ============================================
INSERT INTO "Persons" ("FirstName", "LastName", "Phone", "NationalityCode", "DocumentNumber", "Gender", "CreatedAt", "UpdatedAt") VALUES
('María', 'González López', '987654321', 'PE', '12345678', false, NOW(), NOW()),
('Carlos', 'Rodríguez Pérez', '987654322', 'PE', '23456789', true, NOW(), NOW()),
('Ana', 'Martínez Silva', '987654323', 'PE', '34567890', false, NOW(), NOW()),
('Luis', 'García Torres', '987654324', 'PE', '45678901', true, NOW(), NOW()),
('Carmen', 'López Vargas', '987654325', 'PE', '56789012', false, NOW(), NOW()),
('Roberto', 'Sánchez Díaz', '987654326', 'PE', '67890123', true, NOW(), NOW()),
('Patricia', 'Hernández Cruz', '987654327', 'PE', '78901234', false, NOW(), NOW()),
('Miguel', 'Jiménez Ramos', '987654328', 'PE', '89012345', true, NOW(), NOW()),
('Laura', 'Morales Castro', '987654329', 'PE', '90123456', false, NOW(), NOW()),
('Daniel', 'Ruiz Mendoza', '987654330', 'PE', '01234567', true, NOW(), NOW()),
('Sofía', 'Vega Herrera', '987654331', 'PE', '11234567', false, NOW(), NOW()),
('Andrés', 'Castillo Flores', '987654332', 'PE', '21234567', true, NOW(), NOW()),
('Gabriela', 'Rojas Aguilar', '987654333', 'PE', '31234567', false, NOW(), NOW()),
('Fernando', 'Paredes Luna', '987654334', 'PE', '41234567', true, NOW(), NOW()),
('Valentina', 'Espinoza Ríos', '987654335', 'PE', '51234567', false, NOW(), NOW()),
('Ricardo', 'Guerrero Vidal', '987654336', 'PE', '61234567', true, NOW(), NOW()),
('Natalia', 'Mejía Cortés', '987654337', 'PE', '71234567', false, NOW(), NOW()),
('Óscar', 'Delgado Peña', '987654338', 'PE', '81234567', true, NOW(), NOW()),
('Isabella', 'Campos Salazar', '987654339', 'PE', '91234567', false, NOW(), NOW()),
('Sebastián', 'Vargas Montes', '987654340', 'PE', '02345678', true, NOW(), NOW()),
('Camila', 'Restrepo Ortiz', '987654341', 'PE', '12345679', false, NOW(), NOW()),
('Alejandro', 'Molina Franco', '987654342', 'PE', '22345678', true, NOW(), NOW()),
('Victoria', 'Serrano Gómez', '987654343', 'PE', '32345678', false, NOW(), NOW()),
('Javier', 'Navarro Ibáñez', '987654344', 'PE', '42345678', true, NOW(), NOW());

-- ============================================
-- INSERTAR USUARIOS CON DATOS DEL PRIMER USUARIO
-- ============================================
DO $$
DECLARE
    user_secret TEXT;
    user_password TEXT;
    user_guid UUID;
BEGIN
    -- Obtener los datos del primer usuario
    SELECT "Password" INTO user_password FROM "Users" WHERE "Id" = 1;
    SELECT "SecretKey" INTO user_secret FROM "Authenticators" 
    WHERE "UserGuid" = (SELECT "UserGuid" FROM "Users" WHERE "Id" = 1);
    
    -- Si no hay primer usuario, usar valores predeterminados
    IF user_password IS NULL THEN
        user_password := '$2a$11$N9qo8uLOickgx2ZMRZoMye8zKnlkYZqKr3mOgE7wGz1QvX0ZxYz2i'; -- password: 123456
    END IF;
    
    IF user_secret IS NULL THEN
        user_secret := 'JBSWY3DPEHPK3PXP'; -- Secret predeterminado
    END IF;
    
    -- Insertar usuarios usando los mismos datos
    INSERT INTO "Users" ("PersonId", "Email", "Password", "PositionId", "Status", "StorageSize", "FolderPath", "UserGuid", "CreatedAt", "UpdatedAt") VALUES
    (2, 'maria.gonzalez@empresa.com', user_password, 1, true, 5368709120, '/users/maria_gonzalez/', gen_random_uuid(), NOW(), NOW()),
    (3, 'carlos.rodriguez@empresa.com', user_password, 2, true, 5368709120, '/users/carlos_rodriguez/', gen_random_uuid(), NOW(), NOW()),
    (4, 'ana.martinez@empresa.com', user_password, 7, true, 5368709120, '/users/ana_martinez/', gen_random_uuid(), NOW(), NOW()),
    (5, 'luis.garcia@empresa.com', user_password, 3, true, 5368709120, '/users/luis_garcia/', gen_random_uuid(), NOW(), NOW()),
    (6, 'carmen.lopez@empresa.com', user_password, 4, true, 5368709120, '/users/carmen_lopez/', gen_random_uuid(), NOW(), NOW()),
    (7, 'roberto.sanchez@empresa.com', user_password, 8, true, 5368709120, '/users/roberto_sanchez/', gen_random_uuid(), NOW(), NOW()),
    (8, 'patricia.hernandez@empresa.com', user_password, 5, true, 5368709120, '/users/patricia_hernandez/', gen_random_uuid(), NOW(), NOW()),
    (9, 'miguel.jimenez@empresa.com', user_password, 6, true, 5368709120, '/users/miguel_jimenez/', gen_random_uuid(), NOW(), NOW()),
    (10, 'laura.morales@empresa.com', user_password, 9, true, 5368709120, '/users/laura_morales/', gen_random_uuid(), NOW(), NOW()),
    (11, 'daniel.ruiz@empresa.com', user_password, 10, true, 5368709120, '/users/daniel_ruiz/', gen_random_uuid(), NOW(), NOW()),
    (12, 'sofia.vega@empresa.com', user_password, 11, true, 5368709120, '/users/sofia_vega/', gen_random_uuid(), NOW(), NOW()),
    (13, 'andres.castillo@empresa.com', user_password, 12, true, 5368709120, '/users/andres_castillo/', gen_random_uuid(), NOW(), NOW()),
    (14, 'gabriela.rojas@empresa.com', user_password, 13, true, 5368709120, '/users/gabriela_rojas/', gen_random_uuid(), NOW(), NOW()),
    (15, 'fernando.paredes@empresa.com', user_password, 14, true, 5368709120, '/users/fernando_paredes/', gen_random_uuid(), NOW(), NOW()),
    (16, 'valentina.espinoza@empresa.com', user_password, 15, true, 5368709120, '/users/valentina_espinoza/', gen_random_uuid(), NOW(), NOW()),
    (17, 'ricardo.guerrero@empresa.com', user_password, 16, true, 5368709120, '/users/ricardo_guerrero/', gen_random_uuid(), NOW(), NOW()),
    (18, 'natalia.mejia@empresa.com', user_password, 17, true, 5368709120, '/users/natalia_mejia/', gen_random_uuid(), NOW(), NOW()),
    (19, 'oscar.delgado@empresa.com', user_password, 18, true, 5368709120, '/users/oscar_delgado/', gen_random_uuid(), NOW(), NOW()),
    (20, 'isabella.campos@empresa.com', user_password, 19, true, 5368709120, '/users/isabella_campos/', gen_random_uuid(), NOW(), NOW()),
    (21, 'sebastian.vargas@empresa.com', user_password, 20, true, 5368709120, '/users/sebastian_vargas/', gen_random_uuid(), NOW(), NOW()),
    (22, 'camila.restrepo@empresa.com', user_password, 21, true, 5368709120, '/users/camila_restrepo/', gen_random_uuid(), NOW(), NOW()),
    (23, 'alejandro.molina@empresa.com', user_password, 22, true, 5368709120, '/users/alejandro_molina/', gen_random_uuid(), NOW(), NOW()),
    (24, 'victoria.serrano@empresa.com', user_password, 23, true, 5368709120, '/users/victoria_serrano/', gen_random_uuid(), NOW(), NOW()),
    (25, 'javier.navarro@empresa.com', user_password, 24, true, 5368709120, '/users/javier_navarro/', gen_random_uuid(), NOW(), NOW());
    
    -- Insertar autenticadores para todos los usuarios nuevos
    INSERT INTO "Authenticators" ("UserGuid", "AuthenticatorToken", "SecretKey", "ExpiresAt", "IsActive", "CreatedAt", "UpdatedAt")
    SELECT "UserGuid", 'TOTP_TOKEN', user_secret, NOW() + INTERVAL '1 year', true, NOW(), NOW()
    FROM "Users" WHERE "Id" > 1;
    
END $$;

-- ============================================
-- ASIGNAR ROLES A USUARIOS
-- ============================================
DO $$
DECLARE
    admin_role_id INTEGER;
    director_role_id INTEGER;
    manager_role_id INTEGER;
    area_chief_role_id INTEGER;
    specialist_role_id INTEGER;
    analyst_role_id INTEGER;
    employee_role_id INTEGER;
    assistant_role_id INTEGER;
    min_user_id INTEGER;
BEGIN
    -- Obtener IDs de roles insertados
    SELECT "Id" INTO admin_role_id FROM "Roles" WHERE "Name" = 'Administrador' LIMIT 1;
    SELECT "Id" INTO director_role_id FROM "Roles" WHERE "Name" = 'Director' LIMIT 1;
    SELECT "Id" INTO manager_role_id FROM "Roles" WHERE "Name" = 'Gerente' LIMIT 1;
    SELECT "Id" INTO area_chief_role_id FROM "Roles" WHERE "Name" = 'Jefe de Área' LIMIT 1;
    SELECT "Id" INTO specialist_role_id FROM "Roles" WHERE "Name" = 'Especialista' LIMIT 1;
    SELECT "Id" INTO analyst_role_id FROM "Roles" WHERE "Name" = 'Analista' LIMIT 1;
    SELECT "Id" INTO employee_role_id FROM "Roles" WHERE "Name" = 'Empleado' LIMIT 1;
    SELECT "Id" INTO assistant_role_id FROM "Roles" WHERE "Name" = 'Asistente' LIMIT 1;
    
    -- Obtener el ID mínimo de usuario (después del primer usuario existente)
    SELECT COALESCE(MIN("Id"), 2) INTO min_user_id FROM "Users" WHERE "Id" > 1;
    
    -- Insertar roles de usuario
    INSERT INTO "UserRoles" ("UserId", "RoleId", "CreatedAt", "UpdatedAt") VALUES
    -- Máximas autoridades -> Director
    (min_user_id, director_role_id, NOW(), NOW()),      -- Gerente General
    (min_user_id + 1, director_role_id, NOW(), NOW()),  -- Director de RRHH
    (min_user_id + 2, director_role_id, NOW(), NOW()),  -- Director de TI
    (min_user_id + 11, director_role_id, NOW(), NOW()), -- Jefe de Finanzas
    (min_user_id + 15, director_role_id, NOW(), NOW()), -- Jefe Legal
    (min_user_id + 17, director_role_id, NOW(), NOW()), -- Jefe de Operaciones
    (min_user_id + 20, director_role_id, NOW(), NOW()), -- Jefe de Planeamiento
    (min_user_id + 22, director_role_id, NOW(), NOW()), -- Auditor Jefe
    
    -- Jefes intermedios -> Gerente
    (min_user_id + 3, manager_role_id, NOW(), NOW()),   -- Jefe de Selección
    (min_user_id + 6, manager_role_id, NOW(), NOW()),   -- Jefe de Desarrollo
    
    -- Especialistas -> Especialista
    (min_user_id + 4, specialist_role_id, NOW(), NOW()), -- Especialista en Capacitación
    (min_user_id + 5, specialist_role_id, NOW(), NOW()), -- Especialista en Selección
    (min_user_id + 9, specialist_role_id, NOW(), NOW()), -- Desarrollador Senior
    (min_user_id + 12, specialist_role_id, NOW(), NOW()), -- Contador General
    (min_user_id + 18, specialist_role_id, NOW(), NOW()), -- Supervisor de Operaciones
    
    -- Analistas -> Analista
    (min_user_id + 8, analyst_role_id, NOW(), NOW()),   -- Analista de Sistemas
    (min_user_id + 13, analyst_role_id, NOW(), NOW()),  -- Analista Financiero
    (min_user_id + 21, analyst_role_id, NOW(), NOW()),  -- Analista de Planeamiento
    
    -- Empleados -> Empleado
    (min_user_id + 10, employee_role_id, NOW(), NOW()), -- Desarrollador Junior
    (min_user_id + 19, employee_role_id, NOW(), NOW()), -- Operario
    (min_user_id + 23, employee_role_id, NOW(), NOW()), -- Auditor Junior
    
    -- Asistentes -> Asistente
    (min_user_id + 7, assistant_role_id, NOW(), NOW()),  -- Asistente de RRHH
    (min_user_id + 14, assistant_role_id, NOW(), NOW()), -- Asistente Contable
    (min_user_id + 16, assistant_role_id, NOW(), NOW()); -- Asistente Legal
    
EXCEPTION 
    WHEN OTHERS THEN
        RAISE NOTICE 'Error al insertar roles de usuario: %', SQLERRM;
END $$;

-- ============================================
-- CONSULTAS DE VERIFICACIÓN
-- ============================================

-- Mostrar estructura jerárquica completa
SELECT 
    a."Name" as "Area",
    p."Name" as "Cargo",
    CASE 
        WHEN p."DirectManagerPositionId" IS NULL THEN 'AUTORIDAD MÁXIMA'
        ELSE dm."Name"
    END as "Jefe_Directo",
    CASE 
        WHEN p."DirectManagerPositionId" IS NULL THEN 'Sin jefe (Autoridad máxima del área)'
        ELSE 'Reporta a: ' || dm."Name"
    END as "Observacion"
FROM "Positions" p
INNER JOIN "Areas" a ON p."AreaId" = a."Id"
LEFT JOIN "Positions" dm ON p."DirectManagerPositionId" = dm."Id"
WHERE NOT p."IsDeleted"
ORDER BY a."Id", p."DirectManagerPositionId" NULLS FIRST, p."Id";

-- Mostrar estado de autoridades máximas por área
SELECT 
    a."Name" as "Area",
    CASE 
        WHEN EXISTS(SELECT 1 FROM "Positions" p WHERE p."AreaId" = a."Id" AND p."DirectManagerPositionId" IS NULL AND NOT p."IsDeleted")
        THEN 'SÍ TIENE autoridad máxima'
        ELSE 'NO TIENE autoridad máxima (disponible para nuevo cargo sin jefe)'
    END as "Estado_Autoridad_Maxima",
    (SELECT p."Name" FROM "Positions" p WHERE p."AreaId" = a."Id" AND p."DirectManagerPositionId" IS NULL AND NOT p."IsDeleted" LIMIT 1) as "Autoridad_Maxima"
FROM "Areas" a
WHERE NOT a."IsDeleted"
ORDER BY a."Id";

-- Estadísticas finales
SELECT 'Datos insertados correctamente' as mensaje;
SELECT COUNT(*) as total_areas FROM "Areas" WHERE NOT "IsDeleted";
SELECT COUNT(*) as total_posiciones FROM "Positions" WHERE NOT "IsDeleted";
SELECT COUNT(*) as total_personas FROM "Persons" WHERE NOT "IsDeleted";
SELECT COUNT(*) as total_usuarios FROM "Users" WHERE NOT "IsDeleted";
SELECT COUNT(*) as total_roles FROM "Roles" WHERE NOT "IsDeleted";
SELECT COUNT(*) as total_tipos_documento FROM "DocumentTypes" WHERE NOT "IsDeleted";
SELECT COUNT(*) as total_user_roles FROM "UserRoles" WHERE NOT "IsDeleted";
SELECT COUNT(*) as total_authenticators FROM "Authenticators" WHERE NOT "IsDeleted";