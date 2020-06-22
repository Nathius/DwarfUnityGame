using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Models.AI.AiStates
{
	public virtual class AiState
	{
        public CommandTypes commandType;

        public AiState()
        {
        }

        public void OnStart()
        {
        }

        public void OnUpdate(float inTimeDelta)
        {
        }

        public void OnExit()
        {
        }
	}
}
