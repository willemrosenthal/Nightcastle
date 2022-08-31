using InControl;
using UnityEngine;


public class PlayerInputs : PlayerActionSet
{
	// mouse
	public PlayerAction Click;
	public PlayerAction RightClick;

	// keyboard
	public PlayerAction Shift;

	// keyboard
	public PlayerAction Space;

	public PlayerAction Command;

	// keyboard
	public PlayerAction Escape;

	// keyboard directions
	public PlayerAction Z;
	public PlayerAction X;
	public PlayerAction P;
	public PlayerAction A;
	public PlayerAction S;
	public PlayerAction B;
	

	// keyboard directions
	public PlayerAction ScrollUp;
	public PlayerAction ScrollDown;

	// face buttons
	public PlayerAction AButton;
	public PlayerAction BButton;
	public PlayerAction XButton;
	public PlayerAction YButton;

	public PlayerAction FaceButton;

	// triggers
	public PlayerAction LeftTrigger;
	public PlayerAction RightTrigger;
	public PlayerOneAxisAction ScrollWheel;

	// bumpers
	public PlayerAction RightBumper;
	public PlayerAction LeftBumper;

	// special buttons
	public PlayerAction Start;
	public PlayerAction Select;

	public PlayerAction Tab;
	public PlayerAction CapsLock;

	// dpad
	public PlayerAction DpadUp;
	public PlayerAction DpadDown;
	public PlayerAction DpadRight;
	public PlayerAction DpadLeft;

	// left stick
	public PlayerAction LSLeft;
	public PlayerAction LSRight;
	public PlayerAction LSUp;
	public PlayerAction LSDown;

	// right stick
	public PlayerAction RSLeft;
	public PlayerAction RSRight;
	public PlayerAction RSUp;
	public PlayerAction RSDown;

	// axis
	public PlayerTwoAxisAction Dpad;
	public PlayerTwoAxisAction RightStick;
	public PlayerTwoAxisAction LeftStick;

	// all buttons
	public PlayerAction allButtons;


	public PlayerInputs()
	{

		Click = CreatePlayerAction ("Click");

		RightClick = CreatePlayerAction ("RightClick");

		Shift = CreatePlayerAction ("Shift");

		Space = CreatePlayerAction ("Space");

		Command = CreatePlayerAction ("Command");

		Escape = CreatePlayerAction ("Escape");

		Z = CreatePlayerAction ("Z");
		X = CreatePlayerAction ("X");
		P = CreatePlayerAction ("P");
		A = CreatePlayerAction ("A");
		S = CreatePlayerAction ("S");
		B = CreatePlayerAction ("B");

		ScrollUp = CreatePlayerAction ("Scroll Up");
		ScrollDown = CreatePlayerAction ("Scroll Down");
		ScrollWheel = CreateOneAxisPlayerAction (ScrollUp, ScrollDown);

		AButton = CreatePlayerAction( "A Button" );
		BButton = CreatePlayerAction( "B Button" );
		XButton = CreatePlayerAction( "X Button" );
		YButton = CreatePlayerAction( "Y Button" );

		FaceButton = CreatePlayerAction( "Face Button" );

		LeftTrigger = CreatePlayerAction( "Left Trigger" );
		RightTrigger = CreatePlayerAction( "Right Trigger" );

		RightBumper = CreatePlayerAction( "Right Bumper" );
		LeftBumper = CreatePlayerAction( "Left Bumper" );

		Start = CreatePlayerAction( "Start" );
		Select = CreatePlayerAction( "Select" );
		
		Tab = CreatePlayerAction( "Tab" );
		CapsLock = CreatePlayerAction( "CapsLock" );

		LSLeft = CreatePlayerAction( "Move Left" );
		LSRight = CreatePlayerAction( "Move Right" );
		LSUp = CreatePlayerAction( "Move Up" );
		LSDown = CreatePlayerAction( "Move Down" );

		RSLeft = CreatePlayerAction( "Aim Left" );
		RSRight = CreatePlayerAction( "Aim Right" );
		RSUp = CreatePlayerAction( "Aim Up" );
		RSDown = CreatePlayerAction( "Aim Down" );

		DpadUp = CreatePlayerAction( "D-pad Up" );
		DpadDown = CreatePlayerAction( "D-pad Down" );
		DpadRight = CreatePlayerAction( "D-pad Right" );
		DpadLeft = CreatePlayerAction( "D-pad Left" );

		Dpad = CreateTwoAxisPlayerAction( DpadLeft, DpadRight, DpadDown, DpadUp );
		RightStick = CreateTwoAxisPlayerAction( RSLeft, RSRight, RSDown, RSUp );
		LeftStick = CreateTwoAxisPlayerAction( LSLeft, LSRight, LSDown, LSUp );

		allButtons = CreatePlayerAction( "All Buttons" );
	}


	public static PlayerInputs CreateWithDefaultBindings()
	{
		var playerActions = new PlayerInputs();

		// How to set up mutually exclusive keyboard bindings with a modifier key.
		// playerActions.Back.AddDefaultBinding( Key.Shift, Key.Tab );
		// playerActions.Next.AddDefaultBinding( KeyCombo.With( Key.Tab ).AndNot( Key.Shift ) );

		playerActions.Click.AddDefaultBinding (Mouse.LeftButton);

		playerActions.RightClick.AddDefaultBinding (Mouse.RightButton);

		playerActions.Shift.AddDefaultBinding (Key.LeftShift);
		playerActions.Shift.AddDefaultBinding (Key.RightShift);
		playerActions.Shift.AddDefaultBinding (Key.Shift);

		playerActions.Space.AddDefaultBinding (Key.Space);

		playerActions.Command.AddDefaultBinding (Key.Command);

		playerActions.Escape.AddDefaultBinding (Key.Backspace);
		playerActions.Escape.AddDefaultBinding (InputControlType.Minus);

		playerActions.Z.AddDefaultBinding (Key.Z);
		playerActions.Z.AddDefaultBinding (Key.Q);
		playerActions.Z.AddDefaultBinding (Key.A);

		playerActions.X.AddDefaultBinding (Key.X);
		playerActions.X.AddDefaultBinding (Key.E);
		playerActions.X.AddDefaultBinding (Key.D);

		playerActions.P.AddDefaultBinding (Key.P);

		playerActions.A.AddDefaultBinding (Key.A);

		playerActions.S.AddDefaultBinding (Key.S);

		playerActions.B.AddDefaultBinding (Key.B);

		playerActions.ScrollUp.AddDefaultBinding (Mouse.PositiveScrollWheel);
		playerActions.ScrollDown.AddDefaultBinding (Mouse.NegativeScrollWheel);
		//playerActions.ScrollWheel.AddDefaultBinding (InputControlType.ScrollWheel);

		playerActions.AButton.AddDefaultBinding( Key.V );
		//playerActions.AButton.AddDefaultBinding( Key.B );
		#if UNITY_SWITCH && !UNITY_EDITOR
		playerActions.AButton.AddDefaultBinding( InputControlType.Action2 );
		#else
		playerActions.AButton.AddDefaultBinding( InputControlType.Action1 );
		#endif

		playerActions.BButton.AddDefaultBinding( Key.C );
		//playerActions.BButton.AddDefaultBinding( Key.A );
		#if UNITY_SWITCH && !UNITY_EDITOR
		playerActions.BButton.AddDefaultBinding( InputControlType.Action1 );
		#else
		playerActions.BButton.AddDefaultBinding( InputControlType.Action2 );
		#endif

		playerActions.XButton.AddDefaultBinding( Key.Tab );
		playerActions.XButton.AddDefaultBinding( InputControlType.Action3 );

		playerActions.YButton.AddDefaultBinding( InputControlType.Action4 );

		playerActions.FaceButton.AddDefaultBinding( InputControlType.Action1 );
		playerActions.FaceButton.AddDefaultBinding( InputControlType.Action2 );
		playerActions.FaceButton.AddDefaultBinding( InputControlType.Action3 );
		playerActions.FaceButton.AddDefaultBinding( InputControlType.Action4 );

		//playerActions.LeftTrigger.AddDefaultBinding( Key.A );
		playerActions.LeftTrigger.AddDefaultBinding( InputControlType.LeftTrigger );

		//playerActions.RightTrigger.AddDefaultBinding( Key.S );
		playerActions.RightTrigger.AddDefaultBinding( InputControlType.RightTrigger );

		playerActions.RightBumper.AddDefaultBinding( InputControlType.RightBumper );

		playerActions.LeftBumper.AddDefaultBinding( InputControlType.LeftBumper );
		

		playerActions.Start.AddDefaultBinding( Key.Return );
		playerActions.Start.AddDefaultBinding( InputControlType.Menu );
		playerActions.Start.AddDefaultBinding( InputControlType.Options );
		playerActions.Start.AddDefaultBinding( InputControlType.Plus );
		playerActions.Start.AddDefaultBinding (Key.Escape);

		playerActions.Select.AddDefaultBinding( Key.RightShift );
		playerActions.Select.AddDefaultBinding( InputControlType.View );

		playerActions.Tab.AddDefaultBinding( Key.Tab );
		playerActions.CapsLock.AddDefaultBinding( Key.CapsLock );

		playerActions.DpadUp.AddDefaultBinding( Key.UpArrow );
		playerActions.DpadDown.AddDefaultBinding( Key.DownArrow );
		playerActions.DpadRight.AddDefaultBinding( Key.RightArrow );
		playerActions.DpadLeft.AddDefaultBinding( Key.LeftArrow );

		playerActions.LSUp.AddDefaultBinding( InputControlType.LeftStickUp );
		playerActions.LSDown.AddDefaultBinding( InputControlType.LeftStickDown );
		playerActions.LSLeft.AddDefaultBinding( InputControlType.LeftStickLeft );
		playerActions.LSRight.AddDefaultBinding( InputControlType.LeftStickRight );

		playerActions.DpadUp.AddDefaultBinding( InputControlType.DPadUp );
		playerActions.DpadDown.AddDefaultBinding( InputControlType.DPadDown );
		playerActions.DpadRight.AddDefaultBinding( InputControlType.DPadRight );
		playerActions.DpadLeft.AddDefaultBinding( InputControlType.DPadLeft );


		//playerActions.DpadUp.AddDefaultBinding( InputControlType.PositiveY );
		//playerActions.DpadDown.AddDefaultBinding( InputControlType.NegativeY );
		//playerActions.DpadRight.AddDefaultBinding( InputControlType.PositiveX );
		//playerActions.DpadLeft.AddDefaultBinding( InputControlType.NegativeX );

		playerActions.DpadUp.AddDefaultBinding( Key.UpArrow );
		playerActions.DpadDown.AddDefaultBinding( Key.DownArrow );
		playerActions.DpadRight.AddDefaultBinding( Key.RightArrow );
		playerActions.DpadLeft.AddDefaultBinding( Key.LeftArrow );

		/*
		playerActions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
		playerActions.Right.AddDefaultBinding( InputControlType.LeftStickRight );
		playerActions.Up.AddDefaultBinding( InputControlType.LeftStickUp );
		playerActions.Down.AddDefaultBinding( InputControlType.LeftStickDown );
		*/

		playerActions.RSLeft.AddDefaultBinding( InputControlType.RightStickLeft );
		playerActions.RSRight.AddDefaultBinding( InputControlType.RightStickRight );
		playerActions.RSUp.AddDefaultBinding( InputControlType.RightStickUp );
		playerActions.RSDown.AddDefaultBinding( InputControlType.RightStickDown );

		/*
		playerActions.Up.AddDefaultBinding( Mouse.PositiveY );
		playerActions.Down.AddDefaultBinding( Mouse.NegativeY );
		playerActions.Left.AddDefaultBinding( Mouse.NegativeX );
		playerActions.Right.AddDefaultBinding( Mouse.PositiveX );
		*/

		playerActions.allButtons.AddDefaultBinding( InputControlType.DPadUp );
		playerActions.allButtons.AddDefaultBinding( InputControlType.DPadDown );
		playerActions.allButtons.AddDefaultBinding( InputControlType.DPadRight );
		playerActions.allButtons.AddDefaultBinding( InputControlType.DPadLeft );
		playerActions.allButtons.AddDefaultBinding( InputControlType.LeftTrigger );
		playerActions.allButtons.AddDefaultBinding( InputControlType.RightTrigger );
		playerActions.allButtons.AddDefaultBinding( InputControlType.RightBumper );
		playerActions.allButtons.AddDefaultBinding( InputControlType.LeftBumper );
		playerActions.allButtons.AddDefaultBinding( InputControlType.Action1 );
		playerActions.allButtons.AddDefaultBinding( InputControlType.Action2 );
		playerActions.allButtons.AddDefaultBinding( InputControlType.Action3 );
		playerActions.allButtons.AddDefaultBinding( InputControlType.Action4 );
		playerActions.allButtons.AddDefaultBinding( InputControlType.Plus );

		playerActions.ListenOptions.IncludeUnknownControllers = true;
		//playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
		//playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
		playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = false;
		playerActions.ListenOptions.MaxAllowedBindings = 1;
		//playerActions.ListenOptions.IncludeMouseButtons = true;
		//playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
		//playerActions.ListenOptions.IncludeMouseButtons = true;
		//playerActions.ListenOptions.IncludeMouseScrollWheel = true;

		playerActions.ListenOptions.OnBindingFound = ( action, binding ) => {
			if (binding == new KeyBindingSource( Key.Escape ))
			{
				action.StopListeningForBinding();
				return false;
			}
			return true;
		};

		playerActions.ListenOptions.OnBindingAdded += ( action, binding ) => {
			Debug.Log( "Binding added... " + binding.DeviceName + ": " + binding.Name );
		};

		playerActions.ListenOptions.OnBindingRejected += ( action, binding, reason ) => {
			Debug.Log( "Binding rejected... " + reason );
		};

		return playerActions;
	}
	

	/*
	public class BindingListenOptions
	{
		/// Include controllers when listening for new bindings.
		public bool IncludeControllers = true;

		/// Include unknown controllers when listening for new bindings.
		public bool IncludeUnknownControllers = false;

		/// Include non-standard controls on controllers when listening for new bindings.
		public bool IncludeNonStandardControls = true;

		/// Include mouse buttons when listening for new bindings.
		public bool IncludeMouseButtons = false;

		/// Include keyboard keys when listening for new bindings.
		public bool IncludeKeys = true;

		/// Treat modifiers (Shift, Alt, Control, etc.) as first class keys instead of modifiers.
		public bool IncludeModifiersAsFirstClassKeys = false;

		/// The maximum number of bindings allowed for the action. 
		/// If a new binding is detected and would cause this number to be exceeded, 
		/// enough bindings are removed to make room before adding the new binding.
		/// When zero (default), no limit is applied.
		public uint MaxAllowedBindings = 0;

		/// The maximum number of bindings of a given type allowed for the action. 
		/// If a new binding is detected and would cause this number to be exceeded, 
		/// enough bindings are removed to make room before adding the new binding.
		/// When zero (default), no limit is applied.
		/// When nonzero, this setting overrides MaxAllowedBindings.
		public uint MaxAllowedBindingsPerType = 0;

		/// Allow bindings that are already bound to any other action in the set.
		public bool AllowDuplicateBindingsPerSet = false;

		/// If an existing duplicate binding exists, remove it before adding the new one.
		/// When <code>true</code>, the value of AllowDuplicateBindingsPerSet is irrelevant.
		public bool UnsetDuplicateBindingsOnSet = false;

		/// If not <code>null</code>, and this binding is on the listening action, this binding
		/// will be replace by the newly found binding.
		public BindingSource ReplaceBinding = null;

		/// This function is called when a binding is found but before it is added.
		/// If this function returns false, then the binding is ignored
		/// and listening for new bindings will continue.
		/// If set to null (default), it will not be called.
		public Func<PlayerAction, BindingSource, bool> OnBindingFound = null;

		/// This action is called after a binding is added.
		/// If set to null (default), it will not be called.
		public Action<PlayerAction, BindingSource> OnBindingAdded = null;

		/// This action is called after a binding is found, but rejected along with 
		/// the reason (BindingSourceRejectionType) why it was rejected.
		/// If set to null (default), it will not be called.
		public Action<PlayerAction, BindingSource, BindingSourceRejectionType> OnBindingRejected = null;
	}
	*/
}

