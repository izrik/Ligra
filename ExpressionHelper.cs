using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra
{
    public static class ExpressionHelper
    {
        public static string[] CollectUnboundVariables(this Expression expr, SolusEnvironment env)
        {
            var vars = new List<string>();

            CollectUnboundVariables(expr, env, vars);

            return vars.ToArray();
        }

        static void CollectUnboundVariables(this Expression expr, SolusEnvironment env, List<string> variableNames)
        {
            if (expr is VariableAccess)
            {
                CollectUnboundVariable(env, variableNames, (expr as VariableAccess).VariableName);
            }
            else if (expr is FunctionCall)
            {
                var func = (expr as FunctionCall);
                foreach (var arg in func.Arguments)
                {
                    arg.CollectUnboundVariables(env, variableNames);
                }
            }
            else if (expr is Literal)
            {
                // no need to do anything
            }
            else if (expr is DerivativeOfVariable)
            {
                CollectUnboundVariable(env, variableNames, (expr as DerivativeOfVariable).Variable);
                CollectUnboundVariable(env, variableNames, (expr as DerivativeOfVariable).LowerVariable);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        static void CollectUnboundVariable(SolusEnvironment env, List<string> variableNames, string variableName)
        {
            if (!env.Variables.ContainsKey(variableName))
            {
                variableNames.Add(variableName);
            }
        }
    }
}

