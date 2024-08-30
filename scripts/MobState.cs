using Godot;

namespace Game.StateMachines
{
    // NOTE: This should basically only contain data/state information like a struct,
    // It will be the child of the Item class which will contain all the functionality
    // and for other systems like an inventory system to enforce the m_IsStackable for example
    public partial class MobState : Node
    {
        // Called when the state is entered
        public virtual MobState OnEnterState(Mob mob)
        {
            // Change the mob's animation state here for example
            return null;
        }

        // NOTE: These Input functions are the preferred way to handle SUDDEN input in Godot;
        // NOT for continuous input (like moving the player character). For continuous input,
        // use _Process() or _PhysicsProcess().
        //
        // !: Checking for sudden input in PhysicsProcess() specifically can lead to missed input events.
        //
        // Usage ex:
        // When pressing a mouse side button to dodge for example, HandleInput logic/code will be run;
        // If you press a keyboard key to dodge instead, HandleKeyboardInput logic/code will be run.
        // ----------------------------------------------------------------------------------
        // For gameplay input, both keyboard and mouse (+controller)
        // If it is handled by the InputMap system, is should be accessible through the Input singleton.
        // returns the new state if the state changes, otherwise returns null
        public virtual MobState HandleInput(Mob mob, InputEvent @event)
        {
            return null;
        }

        // returns the new state if the state changes, otherwise returns null
        // Only called if the input event is a keyboard event.
        // NOTE: To handle keyboard events, use this function instead for performance reasons.
        public virtual MobState HandleKeyboardInput(Mob mob, InputEvent @event)
        {
            return null;
        }
        // ----------------------------------------------------------------------------------

        // Called during the processing step of the main loop. 
        // Processing happens at every frame and as fast as possible,
        // so the `delta` time since the previous frame is not constant (`delta` is in seconds).
        // returns the new state if the state changes, otherwise returns null
        public virtual MobState Process(Mob mob, double delta)
        {
            return null;
        }

        // Called during the physics processing step of the main loop.
        // Physics processing means that the frame rate is synced to the physics,
        // i.e. the `delta` variable should be constant (`delta` is in seconds).
        // returns the new state if the state changes, otherwise returns null
        public virtual MobState PhysicsProcess(Mob mob, ref Vector3 velocity, double delta)
        {
            return null;
        }

        // Called when the state is exited - Clean up any state-specific resources here
        public virtual void OnExitState(Mob mob)
        {
            // Clean up any state-specific resources here
        }
    }
}
