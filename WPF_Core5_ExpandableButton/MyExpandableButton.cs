using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPF_Core5_ExpandableButton
{
    public class MyExpandableButton : Button
    {
        private MyExpandableButtonAutomationPeer peer;

        // This example sample gives the button an initial state of collapsed.
        private ExpandCollapseState state = ExpandCollapseState.Collapsed;

        // Add a property for the current expanded/collapse state of the button.
        public ExpandCollapseState State
        {
            get
            {
                return this.state;
            }
            set
            {
                ExpandCollapseState previousState = this.state;

                this.state = value;

                // Raise a UIA property changed event if the expanded state of button has changed.
                if ((this.peer != null) && (this.state != previousState))
                {
                    this.peer.RaisePropertyChangedEvent(
                        ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                        previousState,
                        this.state);
                }

                // IMPORTANT: Take action here to update the button's visuals to match the new state.
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            if (this.peer == null)
            {
                this.peer = new MyExpandableButtonAutomationPeer(this);
            }

            return this.peer;
        }

        protected override void OnClick()
        {
            // In response to the button being tapped or clicked, or invoked 
            // with the keyboard, change the expanded state of the button.
            this.State = (this.State == ExpandCollapseState.Collapsed ?
                ExpandCollapseState.Expanded :
                ExpandCollapseState.Collapsed);
        }
    }

    // Add support for the UIA ExpandCollapse pattern to the standard WPF Button control.
    public class MyExpandableButtonAutomationPeer :
        ButtonAutomationPeer,
        IExpandCollapseProvider
    {
        private MyExpandableButton button;

        public MyExpandableButtonAutomationPeer(MyExpandableButton owner) :
            base(owner)
        {
            this.button = owner;
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            // While were here, prevent the text inside the button from being
            // exosed through UIA. Everything the customer needs it accessible
            // directly through the button.
            return null;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                // Say this control does support the UIA ExpandCollapse pattern.
                return this;
            }
            else if (patternInterface == PatternInterface.Invoke)
            {
                // If semantically this button is only expandable and collapsable, 
                // and not invokable, don't permit it to be programmatically invoked.
                return null;
            }

            return base.GetPattern(patternInterface);
        }

        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return this.button.State;
            }
        }

        public void Expand()
        {
            this.button.State = ExpandCollapseState.Expanded;
        }

        public void Collapse()
        {
            this.button.State = ExpandCollapseState.Collapsed;
        }
    }
}
