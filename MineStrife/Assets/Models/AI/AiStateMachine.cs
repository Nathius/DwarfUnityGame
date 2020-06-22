using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.AI.AiStates;

namespace Assets.Models.AI
{
    class AiStateMachine
    {
        //define a map for each of the start, update draw and end states
        private Dictionary<CommandTypes, AiState> aiStates;

        private AiState currentState;


        public AiStateMachine()
        {
            aiStates = new Dictionary<CommandTypes, AiState>();
        }

        public void addState(CommandTypes commandType, AiState state)
        {
            //if state is not already in state list
            if (!aiStates.ContainsKey(commandType))
            {
                aiStates.Add(commandType, state);
            }
            
        }

        public void changeState(AiState newState)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }

            if (newState != null)
            {
                currentState = newState;

                newState.OnStart();
            }
        }

        public void update(float inTimeDelta)
        {
            if (currentState != null)
            {
                currentState.OnUpdate(inTimeDelta);
            }
        }

        public AiState getCurrentState()
        {
            return currentState;
        }

        public string toString()
        {
            string str = "stateMachine( ";

            if (currentState != null)
            {
                str += currentState.commandType.ToString();
            }
            else
            {
                str += "no state";
            }

            str += " )";

             return str;
        }


    }
}
