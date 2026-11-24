-- =====================================================================
-- PrestQR · Datos de prueba
-- Ejecutar UNA sola vez, después de database/schema.sql, en una base de datos vacía.
--
-- Usuarios de prueba (todos con la contraseña: PrestQR-Demo-2026)
--   admin@prestqr.test    → administrador
--   alumno1@prestqr.test  → alumno (matrícula 2026-0001)
--   alumno2@prestqr.test  → alumno (matrícula 2026-0002)
--
-- Son datos SOLO para desarrollo y revisión local. No usarlos en producción.
-- Para otra contraseña: dotnet fsi scripts/generar-hash.fsx "OtraContraseña"
-- =====================================================================

BEGIN;

-- Administrador
INSERT INTO public.usuarios (email, password_hash, rol, activo)
VALUES ('admin@prestqr.test', '$2a$11$9b2uxYJVOgxZjJ7Zw850ruIUJvonwZknrC42r1kba2YdkAEZ1Kl0m', 'admin', true);

-- Alumnos (la función crea el usuario y el alumno en una sola transacción)
SELECT public.fn_crear_alumno('alumno1@prestqr.test', '$2a$11$9b2uxYJVOgxZjJ7Zw850ruIUJvonwZknrC42r1kba2YdkAEZ1Kl0m', '2026-0001', 'Ana López Ruiz', 'Ingeniería en Software');
SELECT public.fn_crear_alumno('alumno2@prestqr.test', '$2a$11$9b2uxYJVOgxZjJ7Zw850ruIUJvonwZknrC42r1kba2YdkAEZ1Kl0m', '2026-0002', 'Luis Pérez Soto', 'Mecatrónica');

-- Equipos (el código es el contenido del QR)
INSERT INTO public.equipos (codigo_qr, nombre, modelo) VALUES
    ('LAP-001', 'Laptop #001',       'Dell Latitude 5440'),
    ('CAM-001', 'Cámara #001',       'Canon EOS Rebel T7'),
    ('ARD-001', 'Kit Arduino #001',  'Arduino Uno R3');

-- Un préstamo activo para probar la devolución (CAM-001 → Ana, vence en 7 días)
SELECT public.fn_registrar_prestamo(
    (SELECT id FROM public.equipos  WHERE codigo_qr = 'CAM-001'),
    (SELECT id FROM public.alumnos  WHERE matricula = '2026-0001'),
    (SELECT id FROM public.usuarios WHERE email = 'admin@prestqr.test'),
    ((now() AT TIME ZONE 'America/Hermosillo')::date + 7),
    'Préstamo de prueba');

COMMIT;
