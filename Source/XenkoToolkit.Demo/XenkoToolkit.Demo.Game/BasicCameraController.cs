using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Events;
using SiliconStudio.Xenko.Input;
using XenkoToolkit.Engine;

namespace XenkoToolkit.Demo
{
    /// <summary>
    /// A script that allows to move and rotate an entity through keyboard, mouse and touch input to provide basic camera navigation.
    /// </summary>
    /// <remarks>
    /// The entity can be moved using W, A, S, D, Q and E, arrow keys or dragging/scaling using multi-touch.
    /// Rotation is achieved using the Numpad, the mouse while holding the right mouse button, or dragging using single-touch.
    /// </remarks>
    public class BasicCameraController : SyncScript
    {
        private const float maximumPitch = MathUtil.PiOverTwo * 0.99f;

        private Vector3 _upVector;
        private Vector3 _translation;
        private float _yaw;
        private float _pitch;

        private Entity _target;

        private readonly EventReceiver<Entity> _targetAcquired
           = new EventReceiver<Entity>(CameraExtensionsDemo.TargetAcquired);


        public Vector3 KeyboardMovementSpeed { get; set; } = new Vector3(5.0f);

        public Vector3 TouchMovementSpeed { get; set; } = new Vector3(40, 40, 20);

        public float SpeedFactor { get; set; } = 5.0f;

        public Vector2 KeyboardRotationSpeed { get; set; } = new Vector2(3.0f);

        public Vector2 MouseRotationSpeed { get; set; } = new Vector2(90.0f, 60.0f);

        public Vector2 TouchRotationSpeed { get; set; } = new Vector2(60.0f, 40.0f);



        public override void Start()
        {
            base.Start();

            // Default up-direction
            _upVector = Vector3.UnitY;

            // Configure touch input
            if (!Platform.IsWindowsDesktop)
            {
                Input.Gestures.Add(new GestureConfigDrag());
                Input.Gestures.Add(new GestureConfigComposite());
            }
        }

        public override void Update()
        {
            if(_targetAcquired.TryReceive(out var targetCandidate))
            {
                _target = targetCandidate;
            }

            ProcessInput();
            UpdateTransform();
        }

        private void ProcessInput()
        {
            _translation = Vector3.Zero;
            _yaw = 0;
            _pitch = 0;

            // Move with keyboard
            if (Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.Up))
            {
                _translation.Z = -KeyboardMovementSpeed.Z;
            }
            else if (Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.Down))
            {
                _translation.Z = KeyboardMovementSpeed.Z;
            }

            if (Input.IsKeyDown(Keys.A) || Input.IsKeyDown(Keys.Left))
            {
                _translation.X = -KeyboardMovementSpeed.X;
            }
            else if (Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Right))
            {
                _translation.X = KeyboardMovementSpeed.X;
            }

            if (Input.IsKeyDown(Keys.Q))
            {
                _translation.Y = -KeyboardMovementSpeed.Y;
            }
            else if (Input.IsKeyDown(Keys.E))
            {
                _translation.Y = KeyboardMovementSpeed.Y;
            }

            // Alternative translation speed
            if (Input.IsKeyDown(Keys.LeftShift) || Input.IsKeyDown(Keys.RightShift))
            {
                _translation *= SpeedFactor;
            }

            // Rotate with keyboard
            if (Input.IsKeyDown(Keys.NumPad2))
            {
                _pitch = KeyboardRotationSpeed.X;
            }
            else if (Input.IsKeyDown(Keys.NumPad8))
            {
                _pitch = -KeyboardRotationSpeed.X;
            }

            if (Input.IsKeyDown(Keys.NumPad4))
            {
                _yaw = KeyboardRotationSpeed.Y;
            }
            else if (Input.IsKeyDown(Keys.NumPad6))
            {
                _yaw = -KeyboardRotationSpeed.Y;
            }

            // Rotate with mouse
            if (Input.IsMouseButtonDown(MouseButton.Right))
            {
                Input.LockMousePosition();
                Game.IsMouseVisible = false;

                _yaw = -Input.MouseDelta.X * MouseRotationSpeed.X;
                _pitch = -Input.MouseDelta.Y * MouseRotationSpeed.Y;
            }
            else
            {
                Input.UnlockMousePosition();
                Game.IsMouseVisible = true;
            }
            
            // Handle gestures
            foreach (var gestureEvent in Input.GestureEvents)
            {
                switch (gestureEvent.Type)
                {
                    // Rotate by dragging
                    case GestureType.Drag:
                        var drag = (GestureEventDrag)gestureEvent;
                        var dragDistance = drag.DeltaTranslation;
                        _yaw = -dragDistance.X * TouchRotationSpeed.X;
                        _pitch = -dragDistance.Y * TouchRotationSpeed.Y;
                        break;

                    // Move along z-axis by scaling and in xy-plane by multi-touch dragging
                    case GestureType.Composite:
                        var composite = (GestureEventComposite)gestureEvent;
                        _translation.X = -composite.DeltaTranslation.X * TouchMovementSpeed.X;
                        _translation.Y = -composite.DeltaTranslation.Y * TouchMovementSpeed.Y;
                        _translation.Z = -(float)Math.Log(composite.DeltaScale + 1) * TouchMovementSpeed.Z;
                        break;
                }
            }
        }

        private void UpdateTransform()
        {
            var elapsedTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;

            _translation *= elapsedTime;
            _yaw *= elapsedTime;
            _pitch *= elapsedTime;

            // Get the local coordinate system
            var rotation = Matrix.RotationQuaternion(Entity.Transform.Rotation);

            // Enforce the global up-vector by adjusting the local x-axis
            var right = Vector3.Cross(rotation.Forward, _upVector);
            var up = Vector3.Cross(right, rotation.Forward);

            // Stabilize
            right.Normalize();
            up.Normalize();

            // Adjust pitch. Prevent it from exceeding up and down facing. Stabilize edge cases.
            var currentPitch = MathUtil.PiOverTwo - (float)Math.Acos(Vector3.Dot(rotation.Forward, _upVector));
            _pitch = MathUtil.Clamp(currentPitch + _pitch, -maximumPitch, maximumPitch) - currentPitch;

            // Move in local coordinates
            //Entity.Transform.Position += Vector3.TransformCoordinate(translation, rotation);
            Entity.Transform.Translate(_translation);

            if(_target != null && Input.IsKeyDown(Keys.LeftCtrl))
            {
                Entity.Transform.LookAt(_target.Transform,Game.GetDeltaTime() * 3);
            }
            else
            {
                // Yaw around global up-vector, pitch and roll in local space
                Entity.Transform.Rotation *= Quaternion.RotationAxis(right, _pitch) * Quaternion.RotationAxis(_upVector, _yaw);
            }
            
        }
    }
}
