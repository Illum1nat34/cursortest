using System;

[Flags]
public enum PerkTag
{
    None        = 0,
    STEALTH     = 1 << 0,
    LOUD        = 1 << 1,
    RUSH        = 1 << 2,
    SCOUT       = 1 << 3,
    GREED       = 1 << 4,
    SURVIVAL    = 1 << 5,
    PVP         = 1 << 6,
    PVE         = 1 << 7,
    UTILITY     = 1 << 8,
    PROTOCOL    = 1 << 9,
    TRIGGER     = 1 << 10,
    GAMBIT      = 1 << 11,
    TACTIC      = 1 << 12,
    ECON        = 1 << 13,
    MOBILITY    = 1 << 14,
    EXTRACT     = 1 << 15
}