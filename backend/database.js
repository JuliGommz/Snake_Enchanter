/*
====================================================================
* database.js - MySQL Database Connection
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-27
* Version: 2.0
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - MySQL connection pool setup
* - XAMPP-compatible defaults
* Human reviewed and approved.
*
* SETUP:
* 1. XAMPP starten (Apache + MySQL)
* 2. schema.sql in phpMyAdmin importieren
* 3. node server.js starten
*
* DEFAULTS (XAMPP Standard):
* Host:     localhost
* Port:     3306
* User:     root
* Password: (leer)
* Database: snake_enchanter
====================================================================
*/

const mysql = require('mysql2/promise');

// ── Verbindungs-Pool ──────────────────────────────────────────────
// Defaults passen zu XAMPP-Standardinstallation — nichts konfigurieren nötig
const pool = mysql.createPool({
    host:             process.env.DB_HOST     || 'localhost',
    port:   parseInt(process.env.DB_PORT)     || 3306,
    user:             process.env.DB_USER     || 'root',
    password:         process.env.DB_PASSWORD || '',        // XAMPP: kein Passwort
    database:         process.env.DB_NAME     || 'snake_enchanter',
    waitForConnections: true,
    connectionLimit:  10,
    queueLimit:       0
});

// ── Verbindung testen ─────────────────────────────────────────────
async function testConnection() {
    try {
        const conn = await pool.getConnection();
        console.log('Database connected: MySQL @ localhost:3306/snake_enchanter');
        conn.release();
    } catch (err) {
        console.error('Database connection FAILED:', err.message);
        console.error('Bitte XAMPP starten und schema.sql in phpMyAdmin importieren.');
        process.exit(1);
    }
}

testConnection();

module.exports = pool;
