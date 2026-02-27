-- ====================================================================
-- Snake Enchanter - Datenbankschema
-- PIP-3 Theme B | SRH Fachschulen | Entwickler: Julian Gomez
-- ====================================================================
--
-- ANLEITUNG (phpMyAdmin):
-- 1. phpMyAdmin öffnen: http://localhost/phpmyadmin
-- 2. Oben auf "SQL" klicken
-- 3. Diesen gesamten Inhalt einfügen
-- 4. "OK" klicken
-- Fertig — Datenbank und Tabelle werden automatisch erstellt.
-- ====================================================================

-- Datenbank erstellen (falls nicht vorhanden)
CREATE DATABASE IF NOT EXISTS snake_enchanter
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE snake_enchanter;

-- Tabelle für Spielsessions
CREATE TABLE IF NOT EXISTS game_sessions (
    id                    INT           PRIMARY KEY AUTO_INCREMENT,
    session_id            VARCHAR(255)  NOT NULL,
    mode_type             ENUM('simple', 'advanced') NOT NULL,
    success               TINYINT(1)    NOT NULL DEFAULT 0,
    completion_time       INT           NOT NULL DEFAULT 0,
    starting_hp           FLOAT         NOT NULL DEFAULT 30,
    ending_hp             FLOAT         NOT NULL DEFAULT 0,
    total_damage_taken    FLOAT         NOT NULL DEFAULT 0,
    total_hp_restored     FLOAT         NOT NULL DEFAULT 0,
    successful_tune_casts INT           NOT NULL DEFAULT 0,
    failed_tune_casts     INT           NOT NULL DEFAULT 0,
    too_early_count       INT           NOT NULL DEFAULT 0,
    too_late_count        INT           NOT NULL DEFAULT 0,
    snake_bite_count      INT           NOT NULL DEFAULT 0,
    fourth_tune_unlocked  TINYINT(1)    NOT NULL DEFAULT 0,
    hearts_remaining      INT           NOT NULL DEFAULT 0,
    timestamp             DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
