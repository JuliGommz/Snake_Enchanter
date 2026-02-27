/*
====================================================================
* server.js - Snake Enchanter REST API
====================================================================
* Project: Snake Enchanter
* Course: PIP-3 Theme B - SRH Fachschulen
* Developer: Julian Gomez
* Date: 2026-02-27
* Version: 1.0
*
* AUTHORSHIP CLASSIFICATION:
* [AI-ASSISTED]
* - Express server setup
* - REST endpoint implementation
* Human reviewed and approved.
*
* ENDPOINTS:
* POST /api/game-session         - Store session stats after a run
* GET  /api/leaderboard          - Top sessions (?mode=simple|advanced)
* GET  /api/player-stats         - Aggregated stats across all sessions
* GET  /api/health               - Server health check
====================================================================
*/

const express = require('express');
const db = require('./database');

const app = express();
const PORT = 3000;

// ── Middleware ────────────────────────────────────────────────────
app.use(express.json());

// CORS: Allow Unity (localhost) to connect
app.use((req, res, next) => {
    res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS');
    res.setHeader('Access-Control-Allow-Headers', 'Content-Type');
    if (req.method === 'OPTIONS') return res.sendStatus(200);
    next();
});

// ── Health Check ──────────────────────────────────────────────────
app.get('/api/health', (req, res) => {
    res.json({ status: 'ok', message: 'Snake Enchanter API running' });
});

// ── POST /api/game-session ────────────────────────────────────────
// Store stats after each game run (Win or Lose)
app.post('/api/game-session', (req, res) => {
    const {
        sessionId,
        modeType,
        success,
        completionTime,
        startingHp,
        endingHp,
        totalDamageTaken,
        totalHpRestored,
        successfulTuneCasts,
        failedTuneCasts,
        tooEarlyCount,
        tooLateCount,
        snakeBiteCount,
        fourthTuneUnlocked,
        heartsRemaining
    } = req.body;

    // Validate required fields
    if (!sessionId || !modeType) {
        return res.status(400).json({ error: 'sessionId and modeType are required' });
    }

    if (!['simple', 'advanced'].includes(modeType)) {
        return res.status(400).json({ error: 'modeType must be "simple" or "advanced"' });
    }

    const stmt = db.prepare(`
        INSERT INTO game_sessions (
            session_id, mode_type, success, completion_time,
            starting_hp, ending_hp, total_damage_taken, total_hp_restored,
            successful_tune_casts, failed_tune_casts,
            too_early_count, too_late_count, snake_bite_count,
            fourth_tune_unlocked, hearts_remaining
        ) VALUES (
            ?, ?, ?, ?,
            ?, ?, ?, ?,
            ?, ?,
            ?, ?, ?,
            ?, ?
        )
    `);

    const result = stmt.run(
        sessionId,
        modeType,
        success ? 1 : 0,
        completionTime ?? 0,
        startingHp ?? 30,
        endingHp ?? 0,
        totalDamageTaken ?? 0,
        totalHpRestored ?? 0,
        successfulTuneCasts ?? 0,
        failedTuneCasts ?? 0,
        tooEarlyCount ?? 0,
        tooLateCount ?? 0,
        snakeBiteCount ?? 0,
        fourthTuneUnlocked ? 1 : 0,
        heartsRemaining ?? 0
    );

    console.log(`[POST] Session saved: ${sessionId} | Mode: ${modeType} | Success: ${success}`);

    res.status(201).json({
        message: 'Session saved',
        id: result.lastInsertRowid
    });
});

// ── GET /api/leaderboard ──────────────────────────────────────────
// Top 10 successful runs sorted by completion time (fastest first)
// Query: ?mode=simple or ?mode=advanced
app.get('/api/leaderboard', (req, res) => {
    const mode = req.query.mode;

    if (mode && !['simple', 'advanced'].includes(mode)) {
        return res.status(400).json({ error: 'mode must be "simple" or "advanced"' });
    }

    let query = `
        SELECT
            id,
            session_id,
            mode_type,
            completion_time,
            successful_tune_casts,
            failed_tune_casts,
            hearts_remaining,
            fourth_tune_unlocked,
            timestamp
        FROM game_sessions
        WHERE success = 1
    `;

    const params = [];
    if (mode) {
        query += ` AND mode_type = ?`;
        params.push(mode);
    }

    query += ` ORDER BY completion_time ASC LIMIT 10`;

    const rows = db.prepare(query).all(...params);

    console.log(`[GET] Leaderboard (mode=${mode ?? 'all'}): ${rows.length} entries`);

    res.json({ leaderboard: rows });
});

// ── GET /api/player-stats ─────────────────────────────────────────
// Aggregated statistics across all sessions
app.get('/api/player-stats', (req, res) => {
    const stats = db.prepare(`
        SELECT
            COUNT(*)                                        AS total_sessions,
            SUM(success)                                    AS total_wins,
            COUNT(*) - SUM(success)                         AS total_losses,
            ROUND(AVG(CASE WHEN success = 1 THEN completion_time END), 1) AS avg_win_time,
            MIN(CASE WHEN success = 1 THEN completion_time END) AS best_time,
            SUM(successful_tune_casts)                      AS total_successful_casts,
            SUM(failed_tune_casts)                          AS total_failed_casts,
            SUM(snake_bite_count)                           AS total_snake_bites,
            SUM(CASE WHEN mode_type = 'simple' THEN 1 ELSE 0 END)   AS simple_sessions,
            SUM(CASE WHEN mode_type = 'advanced' THEN 1 ELSE 0 END) AS advanced_sessions,
            SUM(CASE WHEN fourth_tune_unlocked = 1 THEN 1 ELSE 0 END) AS fourth_tune_unlocks
        FROM game_sessions
    `).get();

    console.log(`[GET] Player stats: ${stats.total_sessions} total sessions`);

    res.json({ stats });
});

// ── Start Server ──────────────────────────────────────────────────
app.listen(PORT, () => {
    console.log(`Snake Enchanter API running at http://localhost:${PORT}`);
    console.log(`Health: http://localhost:${PORT}/api/health`);
});
