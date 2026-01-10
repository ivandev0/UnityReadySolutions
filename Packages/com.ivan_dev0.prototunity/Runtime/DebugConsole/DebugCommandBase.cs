using System;
using JetBrains.Annotations;

namespace PrototUnity.DebugConsole {
	public abstract class DebugCommandBase {
		public string CommandName { get; private set; }
		public string CommandDescription { get; private set; }

		protected DebugCommandBase(string commandName, string commandDescription) {
			CommandName = commandName;
			CommandDescription = commandDescription;
		}
	}
	
	public class DebugCommand : DebugCommandBase {
		[NotNull] private readonly Action action;
		
		public DebugCommand(string commandName,
			string commandDescription,
			[NotNull] Action action
		) : base(commandName, commandDescription) {
			this.action = action;
		}

		public void Invoke() {
			action.Invoke();
		}
	}
	
	public class DebugCommandWithOneArg : DebugCommandBase {
		[NotNull] private readonly Action<string> action;
		
		public DebugCommandWithOneArg(string commandName,
			string commandDescription,
			[NotNull] Action<string> action
		) : base(commandName, commandDescription) {
			this.action = action;
		}

		public void Invoke(string arg) {
			action.Invoke(arg);
		}
	}
	
	public class DebugCommandWithTwoArgs : DebugCommandBase {
		[NotNull] private readonly Action<string, string> action;
		
		public DebugCommandWithTwoArgs(string commandName,
			string commandDescription,
			[NotNull] Action<string, string> action
		) : base(commandName, commandDescription) {
			this.action = action;
		}

		public void Invoke(string arg1, string arg2) {
			action.Invoke(arg1, arg2);
		}
	}
}