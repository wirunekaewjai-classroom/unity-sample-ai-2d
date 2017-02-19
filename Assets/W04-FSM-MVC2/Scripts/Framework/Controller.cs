﻿using System.Collections.Generic;
using UnityEngine;

namespace Wirune.W04
{
    public class Controller<TModel, TView> : MonoBehaviour
        where TModel : IModel
        where TView : IView
    {
        [SerializeField]
        private TModel m_Model;
        public TModel Model { get { return m_Model; } }

        [SerializeField]
        private TView m_View;
        public TView View { get { return m_View; } }

        private Dictionary<object, ControllerState<TModel, TView>> m_States;
        private object m_CurrentStateID;

        private Stack<object> m_PreviousStatesIDs;

        private void Awake()
        {
            m_States = new Dictionary<object, ControllerState<TModel, TView>>();
            m_CurrentStateID = null;

            m_PreviousStatesIDs = new Stack<object>();

            OnAwake();
        }

        private void Start()
        {
            OnStart();
            Model.OnLoad();
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnStart()
        {
            
        }

        protected void CreateState<TState>(object stateID)
            where TState : ControllerState<TModel, TView>
        {
            var state = System.Activator.CreateInstance<TState>();

            state.Controller = this;
            state.OnCreate();

            m_States.Add(stateID, state);
        }

        public void ChangeState(object nextStateID)
        {
            if (m_CurrentStateID != null)
            {
                m_States[m_CurrentStateID].OnExit();
                m_PreviousStatesIDs.Push(m_CurrentStateID);
            }

            m_CurrentStateID = nextStateID;

            if (m_CurrentStateID != null)
            {
                m_States[m_CurrentStateID].OnEnter();
            }
        }

        public bool GoToPreviousState()
        {
            if (m_PreviousStatesIDs.Count > 0)
            {
                object previousStateID = m_PreviousStatesIDs.Pop();

                if (m_CurrentStateID != null)
                {
                    m_States[m_CurrentStateID].OnExit();
                }

                m_CurrentStateID = previousStateID;

                if (m_CurrentStateID != null)
                {
                    m_States[m_CurrentStateID].OnEnter();
                }

                return true;
            }

            return false;
        }


        public void ClearPreviousStates()
        {
            m_PreviousStatesIDs.Clear();
        }
    }
}

