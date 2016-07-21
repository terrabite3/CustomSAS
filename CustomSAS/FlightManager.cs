using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomSAS
{
    class FlightManager
    {
        internal void OnPreAutopilotUpdate(FlightCtrlState st)
        {
        }

        internal void OnAutopilotUpdate(FlightCtrlState st)
        {
        }

        internal void OnPostAutopilotUpdate(FlightCtrlState st)
        {
        }

        internal void OnFlyByWire(FlightCtrlState st)
        {
            if (st.isNeutral)
                st.pitch = 0.5f;
        }

        internal void Reset()
        {
            //print("Reset");
        }
    }
}
