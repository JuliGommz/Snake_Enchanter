/*
====================================================================
* database.js - SQLite Database Setup
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-27
* Version: 1.0
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Database schema design
* - SQLite initialization
* Human reviewed and approved.
====================================================================
*/

const Database = require('better-sqlite3');
const path = require('path');

// Database file lives next to this script
const DB_PATH = path.join(__dirname, 'snake_enchanter.db');

const db = new Database(DB_PATH);

// Enable WAL mode for better performance
db.pragma('journal_mode = WAL');

// Create table if it doesn't exist
db.exec(`
    CREATE TABLE IF NOT EXISTS game_sessions (
        id                    INTEGER PRIMARY KEY AUTOINCREMENT,
        session_id            TEXT    NOT NULL,
        mode_type             TEXT    NOT NULL CHECK(mode_type IN ('simple', 'advanced')),
        success               INTEGER NOT NULL DEFAULT 0,
        completion_time       INTEGER NOT NULL DEFAULT 0,
        starting_hp           REAL    NOT NULL DEFAULT 30,
        ending_hp             REAL    NOT NULL DEFAULT 0,
        total_damage_taken    REAL    NOT NULL DEFAULT 0,
        total_hp_restored     REAL    NOT NULL DEFAULT 0,
        successful_tune_casts INTEGER NOT NULL DEFAULT 0,
        failed_tune_casts     INTEGER NOT NULL DEFAULT 0,
        too_early_count       INTEGER NOT NULL DEFAULT 0,
        too_late_count        INTEGER NOT NULL DEFAULT 0,
        snake_bite_count      INTEGER NOT NULL DEFAULT 0,
        fourth_tune_unlocked  INTEGER NOT NULL DEFAULT 0,
        hearts_remaining      INTEGER NOT NULL DEFAULT 0,
        timestamp             TEXT    NOT NULL DEFAULT (datetime('now'))
    )
`);

console.log(`Database ready: ${DB_PATH}`);

module.exports = db;
