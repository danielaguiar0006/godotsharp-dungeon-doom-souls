using Godot;

struct InputActions
{
    // Static Strings For Garbage Collection Optimization - Godot uses custom strings for actions which causes potential GC problems
    public static StringName s_MoveForward = new StringName("move_forward");
    public static StringName s_MoveBackward = new StringName("move_backward");
    public static StringName s_MoveLeft = new StringName("move_left");
    public static StringName s_MoveRight = new StringName("move_right");
    public static StringName s_MoveJump = new StringName("move_jump");
    public static StringName s_MoveDodge = new StringName("move_dodge");
    public static StringName s_AttackOne = new StringName("attack_one");
}
