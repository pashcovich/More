﻿namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;
    using global::Windows.UI.Xaml.Media.Animation;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class FrameShellViewBase
    {
        private TransitionCollection transitions;

        partial void BeforeFirstNavigation( Frame frame )
        {
            if ( frame == null || frame.ContentTransitions == null )
                return;

            transitions = new TransitionCollection();
            transitions.AddRange( frame.ContentTransitions );

            frame.ContentTransitions = null;
            frame.Navigated += OnFirstNavigation;
        }

        private void OnFirstNavigation( object sender, NavigationEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            var frame = (Frame) sender;

            frame.Navigated -= OnFirstNavigation;
            frame.ContentTransitions = transitions ?? new TransitionCollection() { new NavigationThemeTransition() };

            transitions = null;
        }
    }
}
