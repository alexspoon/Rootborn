using Godot;
using System;
using System.Threading.Tasks;

public partial class MovementMidair : PlayerState
{
    public MovementMidair(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }
    public bool JumpBuffered;
    public bool CoyoteJump;
    public State Previous;
    public float PreviousYVelocity;
    public float LocalGravity;
    private GpuParticles2D particleTrail;

    public override void Enter(State previous = null)
    {
        Previous = previous;
        //Particle jump effect
        particleTrail = controller.particleTrail;
        if (previous is MovementJump)
        {
            var jump = previous as MovementJump;
            if (jump.Previous is MovementIdle or MovementWalking)
            {
                particleTrail.Emitting = true;
                var particleTimer = UtilityFunctions.CreateOneShotTimer(0.025f, player);
                particleTimer.Timeout += () =>
                {
                    particleTrail.Emitting = false;
                    particleTimer.QueueFree();
                };
            }
        }
        else particleTrail.Emitting = false;

        LocalGravity = controller.FallAceleration;
        CoyoteJump = false;
        JumpBuffered = false;
        GD.Print(previous);
        if (previous is not (MovementJump or MovementWallJump or MovementWallGrab)) CoyoteTime();
        if (previous is MovementPogo)
        {
            LocalGravity -= 5;
        }
        controller.MaxSpeed = 250;
    }

    public override void Exit()
    {
        controller.MaxSpeed = 220;
    }

    public override void PhysicsProcess(float delta)
    {
        if (Previous is MovementDash)
        {
            player.Mesh.RotationDegrees = Mathf.Lerp(player.Mesh.RotationDegrees, Mathf.RoundToInt(controller.HorizontalInput) * 15f, 0.25f);
            controller.VelocityChange = controller.GroundDeceleration;
        }
        else if (controller.HorizontalInput == 0)
        {
            player.Mesh.RotationDegrees = Mathf.Lerp(player.Mesh.RotationDegrees, 0, 0.25f);
            controller.VelocityChange = controller.AirDeceleration;
        }
        else
        {
            player.Mesh.RotationDegrees = Mathf.Lerp(player.Mesh.RotationDegrees, Mathf.RoundToInt(controller.HorizontalInput) * 15f, 0.25f);
            controller.VelocityChange = controller.AirAcceleration;
        }


        if (CoyoteJump && Input.IsActionJustPressed("inputJump"))
        {
            StateMachine.ChangeState("Jump");
            return;
        }
        else if (Input.IsActionJustPressed("inputDash") && controller.HorizontalInput != 0) StateMachine.ChangeState("Dash");
        else if (Input.IsActionJustPressed("inputJump")) JumpBuffer();
        else if (player.IsOnWallOnly() && (Previous is MovementWallJump || (Mathf.RoundToInt(controller.HorizontalInput) != 0 && controller.TargetVelocity.Y > 0))) StateMachine.ChangeState("WallGrab");
        else if (player.IsOnFloor())
        {
            if (JumpBuffered) StateMachine.ChangeState("Jump");
            else if (controller.HorizontalInput == 0) StateMachine.ChangeState("Idle");
            else StateMachine.ChangeState("Walking");
        }
        controller.TargetVelocity.Y += LocalGravity;
        controller.TargetVelocity = new Vector2(controller.TargetVelocity.X, Mathf.Min(controller.TargetVelocity.Y, controller.TerminalVelocity));
        if (Previous is MovementJump && PreviousYVelocity < 0.01 && controller.TargetVelocity.Y > 0)
        {
            GD.Print("apex");
            LocalGravity = controller.FallAceleration / 4;
            controller.MaxSpeed = 280;
        }
        else if (Previous is not MovementPogo)
        {
            LocalGravity = controller.FallAceleration;
            controller.MaxSpeed = 250;
        } 
        if (player.IsOnCeilingOnly()) controller.TargetVelocity.Y = controller.FallAceleration;
        PreviousYVelocity = controller.TargetVelocity.Y;
    }

    private void JumpBuffer()
    {
        JumpBuffered = true;
        var bufferTimer = UtilityFunctions.CreateOneShotTimer(controller.JumpBufferTime, player);
        bufferTimer.Timeout += () =>
        {
            JumpBuffered = false;
            bufferTimer.QueueFree();
        };
    }

    private void CoyoteTime()
    {
        CoyoteJump = true;
        var coyoteTimer = UtilityFunctions.CreateOneShotTimer(controller.CoyoteTimeDuration, player);
        coyoteTimer.Timeout += () =>
        {
            CoyoteJump = false;
            coyoteTimer.QueueFree();
        };
    }
}
