using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBx;

namespace IBx.AI
{
    public class ScriptedAttacker : IModel
    {
        private string m_aiScript;
        
        public void InvokeAI(GameView gv, ScreenCombat sc, Creature crt)
        {
            if (gv.mod.debugMode)
            {
                gv.cc.addLogText("<font color='red'>" + crt.cr_name + " <font color='white'>is a ScriptedAttacker</font><BR>");
            }

            // Collect a ton of information here that is relevant to the AI Script like closest PC,
            // damage of weapons
            Scripting.ScriptInputs scriptInputs = buildScriptInputs(gv, sc, crt);

            // Grab the script engine and pass in the parameters as well as the script (crt.aiScript)
            Scripting.ScriptEngine engine = Scripting.ScriptEngine.getEngine();

            // Run the script and grab the outputs
            string scriptContent = null;
            try
            {
                // This lookup needs to change to be more consistent with the way that files are fetched
                string fullPath = System.IO.Directory.GetParent(GetType().Assembly.Location).Parent.ToString() + "\\Assets\\ibscript\\" + crt.ai_script;
                scriptContent = System.IO.File.ReadAllText(fullPath);
            } catch (Exception ex)
            {
            }
            if (scriptContent == null)
            {
                //todo - log or throw exception
                new BasicAttacker().InvokeAI(gv, sc, crt);
                return;
            }
            Scripting.ScriptOutputs scriptOutputs = engine.RunScript(scriptContent, scriptInputs);

            // Check the outputs for validity - todo
            // Make sure that scriptOutputs["ActionToTake"] is not null and that the "ActionToTake"
            // key is in the dictionary and that it is in the allowed values
            // If we are going to use player names, check them here against the roster for validity 

            // Set necessary parameters on GV and SC objects based on the output
            gv.sf.ActionToTake = scriptOutputs["ActionToTake"];
            gv.sf.CombatTarget = sc.GetPlayerByName(scriptOutputs["CombatTarget"]);

        }

        private string getScript(string aiScriptPath)
        {
            // Note: This will cache the script so changing the script at runtime will NOT work
            if (this.m_aiScript == null) {
            System.IO.FileStream scriptStream = new System.IO.FileStream(aiScriptPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.StreamReader sr = new System.IO.StreamReader(scriptStream);
                m_aiScript = sr.ReadToEnd();
            }
            return m_aiScript;
        }

        private Scripting.ScriptInputs buildScriptInputs(GameView gv, ScreenCombat sc, Creature crt)
        {
            IBx.Scripting.ScriptInputs scriptInputs = new Scripting.ScriptInputs();
            Player pc = sc.targetClosestPC(crt);
            scriptInputs["closestPC"] = pc.name;
            scriptInputs["cr_category"] = crt.cr_category;

            List<AITarget> targets = new List<AITarget>();
            int idxTarget = 0;
            foreach (Player p in gv.mod.playerList) {
                AITarget target = new AITarget();
                target.distance = sc.CalcDistance(crt, crt.combatLocX, crt.combatLocY, p.combatLocX, p.combatLocY);
                target.ac = p.AC;
                target.hp = p.hp;
                target.hpPercentage = p.hp / p.hpMax;
                target.name = p.name;
                target.playerClass = p.playerClass.name;
                targets.Add(target);
                idxTarget++;
            }
            scriptInputs["targets"] = targets.ToArray();

            return scriptInputs;
        }
    }
}
