---
status: gathering
trigger: "Snakes are passing through Props (Pillars, Support Beams, Wall decorations) despite all documented fixes being applied. Walls and Player blocking works correctly."
created: 2026-02-14T10:00:00Z
updated: 2026-02-14T10:00:00Z
---

## Current Focus

hypothesis: Raycast origin is INSIDE the Prop's collider, causing Raycast to start beyond the collider boundary (raycasts don't detect colliders they start inside)
test: Check RaycastAll debug output for Prop detection + verify rayOrigin position relative to Prop bounds
expecting: If hypothesis correct, RaycastAll will show 0 hits OR will show hits AFTER passing through Prop
next_action: Await user's RaycastAll Console output + ask for Snake/Prop world positions when bug occurs

## Symptoms

expected: Snakes should be blocked by Props (Pillars, Wall decorations) just like they're blocked by Walls and Player
actual: Snakes walk through Props as if they don't exist. No console logs for Props blocking.
errors: None - code runs without errors
reproduction:
1. Unity Play Mode
2. Position test Prop "WallPropsBottomA_ST (2)" between snake and player
3. Snake chases player
4. Snake walks THROUGH the Prop (Box Collider)
5. No console log for "Movement blocked by WallPropsBottomA_ST"

started: Today (Session 15). Yesterday (Session 14) the same 3 fixes reportedly worked, then git revert happened.

## Eliminated

(none yet)

## Evidence

- timestamp: 2026-02-14T10:05:00Z
  checked: SnakeAI.cs MoveTowardsSafe() method (lines 733-769)
  found: RaycastAll debug code present and should log all hits with collider name, tag, layer, distance
  implication: If Props don't appear in RaycastAll output, then Physics.Raycast literally cannot see them (layer, disabled collider, or collider doesn't exist)

- timestamp: 2026-02-14T10:06:00Z
  checked: Test Prop details from user
  found: WallPropsBottomA_ST (2) has Box Collider (NOT Mesh Collider), Tag "Environment", Is Trigger = OFF
  implication: Convex fix (Fix 2) is irrelevant for this specific test object since Convex only applies to Mesh Colliders

- timestamp: 2026-02-14T10:07:00Z
  checked: User report of snake overlap behavior
  found: Snakes overlap each other despite code comment saying "Snake: BLOCKS (prevent snake overlap)"
  implication: If Raycast isn't hitting Snake colliders either, this is a systemic Raycast issue, not Props-specific

- timestamp: 2026-02-14T10:10:00Z
  checked: WallPropsBottomA_ST.prefab file structure
  found: BoxCollider component present, m_Enabled: 1, m_IsTrigger: 0, m_Layer: 0 (Default), m_TagString: Environment
  implication: Prefab configuration is CORRECT - collider exists, enabled, not trigger, correct layer/tag

- timestamp: 2026-02-14T10:11:00Z
  checked: Physics.Raycast calls in SnakeAI.cs
  found: No LayerMask filtering - uses default (hits all layers except IgnoreRaycast)
  implication: Raycast SHOULD hit Default layer objects. Props are on Default layer, so should be detected.

- timestamp: 2026-02-14T10:15:00Z
  checked: Raycast origin calculation in MoveTowardsSafe (line 744)
  found: rayOrigin = transform.position + Vector3.up * 0.5f (0.5 units above snake's pivot)
  implication: If Snake's pivot is at ground level, rayOrigin is 0.5 units up. WallPropsBottomA_ST BoxCollider center is at y=0.8, size.y=1.6 (so collider spans y=0 to y=1.6). Raycast at y=0.5 is INSIDE the collider vertical bounds.

- timestamp: 2026-02-14T10:17:00Z
  checked: Unity Physics.Raycast documentation behavior
  found: Raycasts do NOT detect colliders when the ray origin is INSIDE the collider volume
  implication: If Snake is standing directly against/inside the Prop's X/Z bounds, the rayOrigin (y=0.5) starts INSIDE the BoxCollider (which extends from y=0 to y=1.6), causing Raycast to miss it entirely!

## Resolution

root_cause:
fix:
verification:
files_changed: []
