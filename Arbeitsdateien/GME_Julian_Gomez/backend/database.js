/*
====================================================================
* database.js - MySQL Database Connection
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-27
* Version: 3.0
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - MySQL connection pool setup
* - XAMPP-compatible defaults
* - Auto-init: DB + table created on first start (no phpMyAdmin needed)
* Human reviewed and approved.
*
* SETUP:
* 1. XAMPP starten (MySQL muss laufen)
* 2. node server.js starten  ← DB wird automatisch angelegt
*
* DEFAULTS (XAMPP Standard):
* Host:     localhost
* Port:     3306
* User:     root
* Password: (leer)
* Database: snake_enchanter  ← wird automatisch erstellt
====================================================================
*/

const mysql = require('mysql2/promise');

const DB_NAME = process.env.DB_NAME || 'snake_enchanter';

const BASE_CONFIG = {
    host:     process.env.DB_HOST     || 'localhost',
    port: parseInt(process.env.DB_PORT) || 3306,
    user:     process.env.DB_USER     || 'root',
    password: process.env.DB_PASSWORD || '',
};

// ── Auto-Init: DB + Tabelle beim ersten Start anlegen ─────────────
// Verbindet zuerst ohne DB-Name, erstellt DB + Tabelle falls nötig.
// Kein manueller phpMyAdmin-Import erforderlich.
async function initDatabase() {
    const conn = await mysql.createConnection(BASE_CONFIG);
    await conn.query(
        `CREATE DATABASE IF NOT EXISTS \`${DB_NAME}\`
         CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci`
    );
    await conn.query(`USE \`${DB_NAME}\``);
    await conn.query(`
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
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
    `);
    await conn.end();
    console.log('Database ready: snake_enchanter');
}

// ── Verbindungs-Pool ──────────────────────────────────────────────
const pool = mysql.createPool({
    ...BASE_CONFIG,
    database:           DB_NAME,
    waitForConnections: true,
    connectionLimit:    10,
    queueLimit:         0
});

// ── Startup: init dann Verbindung testen ──────────────────────────
async function startup() {
    try {
        await initDatabase();
        const conn = await pool.getConnection();
        console.log('Database connected: MySQL @ localhost:3306/snake_enchanter');
        conn.release();
    } catch (err) {
        console.error('Database startup FAILED:', err.message);
        console.error('→ Bitte XAMPP öffnen und MySQL starten, dann erneut versuchen.');
        process.exit(1);
    }
}

startup();

module.exports = pool;
