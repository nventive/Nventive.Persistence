﻿using System;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.Activation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Nventive.Persistence.Sample
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		private Window _window;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			InitializeLogging();

			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

#if NET7_0_OR_GREATER && WINDOWS && !HAS_UNO
			_window = new Window();
			_window.Activate();
#else
			_window = Microsoft.UI.Xaml.Window.Current;
#endif

			var rootFrame = _window.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (args.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					// TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				_window.Content = rootFrame;
			}

#if !(NET7_0_OR_GREATER && WINDOWS)
			if (args.UWPLaunchActivatedEventArgs.PrelaunchActivated == false)
#endif
			{
				if (rootFrame.Content == null)
				{
					// When the navigation stack isn't restored navigate to the first page,
					// configuring the new page by passing required information as a navigation
					// parameter
					rootFrame.Navigate(typeof(MainPage), args.Arguments);
				}
				// Ensure the current window is active
				_window.Activate();
			}
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
		}

		/// <summary>
		/// Configures global Uno Platform logging
		/// </summary>
		private static void InitializeLogging()
		{
#if DEBUG
			// Logging is disabled by default for release builds, as it incurs a significant
			// initialization cost from Microsoft.Extensions.Logging setup. If startup performance
			// is a concern for your application, keep this disabled. If you're running on web or 
			// desktop targets, you can use url or command line parameters to enable it.
			//
			// For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

			var factory = LoggerFactory.Create(builder =>
			{
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
				builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
				builder.AddDebug();
#else
				builder.AddConsole();
#endif

				// Exclude logs below this level
				builder.SetMinimumLevel(LogLevel.Information);

				// Default filters for Uno Platform namespaces
				builder.AddFilter("Uno", LogLevel.Warning);
				builder.AddFilter("Windows", LogLevel.Warning);
				builder.AddFilter("Microsoft", LogLevel.Warning);

				// Generic Xaml events
				// builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
				// builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
				// builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
				// builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
				// builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

				// Layouter specific messages
				// builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
				// builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
				// builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

				// builder.AddFilter("Windows.Storage", LogLevel.Debug );

				// Binding related messages
				// builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
				// builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

				// Binder memory references tracking
				// builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

				// RemoteControl and HotReload related
				// builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

				// Debug JS interop
				// builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
			});

			global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
			global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
		}
	}
}
