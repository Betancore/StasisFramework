﻿using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using ProductX.Framework.Enums;

namespace ProductX.Framework.Attributes
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class CustomRetryAttribute : PropertyAttribute, IWrapSetUpTearDown
	{
		private readonly int _count;

		/// <summary>
		/// Construct a RepeatAttribute.
		/// </summary>
		/// <param name="count">The number of times to run the test.</param>
		public CustomRetryAttribute([Optional]int count) : base(count)
		{
			_count = count == 0 ? int.Parse(RunConfigurator.GetValue(RunValues.RetryCount)) : count;
		}

		#region IWrapSetUpTearDown Members

		/// <summary>
		/// Wrap a command and return the result.
		/// </summary>
		/// <param name="command">The command to be wrapped.</param>
		/// <returns>The wrapped command.</returns>
		public TestCommand Wrap(TestCommand command)
		{
			return new CustomRetryCommand(command, _count);
		}

		#endregion

		#region Nested CustomRetryAttribute Class

		/// <summary>
		/// The test command for the RetryAttribute.
		/// </summary>
		public class CustomRetryCommand : DelegatingTestCommand
		{
			private readonly int _retryCount;

			/// <summary>
			/// Initializes a new instance of the <see cref="CustomRetryCommand"/> class.
			/// </summary>
			/// <param name="innerCommand">The inner command.</param>
			/// <param name="retryCount">The number of repetitions.</param>
			public CustomRetryCommand(TestCommand innerCommand, int retryCount) : base(innerCommand)
			{
				_retryCount = retryCount;
			}

			/// <summary>
			/// Runs the test, saving a TestResult in the supplied TestExecutionContext.
			/// </summary>
			/// <param name="context">The context in which the test should run.</param>
			/// <returns>A TestResult.</returns>
			public override TestResult Execute(TestExecutionContext context)
			{
				var count = _retryCount;

				while (count-- > -1)
				{
					context.CurrentResult = innerCommand.Execute(context);
					var results = context.CurrentResult.ResultState;

					if (!Equals(results, ResultState.Error)
						&& !Equals(results, ResultState.Failure)
						&& !Equals(results, ResultState.SetUpError)
						&& !Equals(results, ResultState.SetUpFailure)
						&& !Equals(results, ResultState.TearDownError)
						&& !Equals(results, ResultState.ChildFailure))
					{
						break;
					}
				}

				return context.CurrentResult;
			}
		}

		#endregion
	}
}