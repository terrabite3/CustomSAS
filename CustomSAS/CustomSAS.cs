using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomSAS
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class CustomSAS : MonoBehaviour
    {
        private FlightManager m_FlightManager = new FlightManager();

        private static CustomSAS m_Instance = null;

        public static CustomSAS Instance
        {
            get
            {
                return m_Instance;
            }
        }
        
        public void Awake()
        {
            m_Instance = this;

            RegisterCallbacks();

            print("CustomSAS: Initialized");
        }

        public void OnDestroy()
        {
            m_Instance = null;
            
            UnregisterCallbacks();

            m_FlightManager = null;

            print("CustomSAS: Deinitialized");
        }

        private void RegisterCallbacks()
        {

            GameEvents.onVesselChange.Add(OnVesselChange);
        }

        private void UnregisterCallbacks()
        {

            GameEvents.onVesselChange.Remove(OnVesselChange);

            if (FlightGlobals.ActiveVessel != null)
            {
                FlightGlobals.ActiveVessel.OnPreAutopilotUpdate -= m_FlightManager.OnPreAutopilotUpdate;
                FlightGlobals.ActiveVessel.OnAutopilotUpdate -= m_FlightManager.OnAutopilotUpdate;
                FlightGlobals.ActiveVessel.OnPostAutopilotUpdate -= m_FlightManager.OnPostAutopilotUpdate;
                FlightGlobals.ActiveVessel.OnFlyByWire -= m_FlightManager.OnFlyByWire;
            }
        }

        void AddFlyByWireCallbackToActiveVessel()
        {
            FlightGlobals.ActiveVessel.OnPreAutopilotUpdate += m_FlightManager.OnFlyByWire;
            m_LastChangedActiveVessel = FlightGlobals.ActiveVessel;

            if (FlightGlobals.ActiveVessel.Autopilot != null && FlightGlobals.ActiveVessel.Autopilot.SAS != null
                && FlightGlobals.ActiveVessel.Autopilot.SAS.CanEngageSAS() && FlightGlobals.ActiveVessel.HasControlSources()
                && !FlightGlobals.ActiveVessel.isEVA)
            {
                FlightGlobals.ActiveVessel.Autopilot.SAS.ConnectFlyByWire();
            }
        }

        private Vessel m_LastChangedActiveVessel = null;

        IEnumerator<YieldInstruction> WaitAndAddFlyByWireCallbackToActiveVessel()
        {
            yield return null;
            AddFlyByWireCallbackToActiveVessel();
        }

        private void OnVesselChange(Vessel vessel)
        {
            if (vessel == null)
            {
                return;
            }

            if (m_LastChangedActiveVessel != null)
            {
                if (m_LastChangedActiveVessel.Autopilot != null && m_LastChangedActiveVessel.Autopilot.SAS != null)
                {
                    FlightGlobals.ActiveVessel.Autopilot.SAS.DisconnectFlyByWire();
                }

                m_LastChangedActiveVessel.OnPreAutopilotUpdate -= m_FlightManager.OnFlyByWire;
                m_LastChangedActiveVessel = FlightGlobals.ActiveVessel;
            }
            
            m_FlightManager.Reset();

            StartCoroutine(WaitAndAddFlyByWireCallbackToActiveVessel());
        }

        private void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
            {
                if (TimeWarp.fetch != null && TimeWarp.fetch.Mode == TimeWarp.Modes.HIGH && TimeWarp.CurrentRateIndex != 0)
                {
                    m_FlightManager.OnFlyByWire(new FlightCtrlState());
                }
            }
        }
        
    }
}
