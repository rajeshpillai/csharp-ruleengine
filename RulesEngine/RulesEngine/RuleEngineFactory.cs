using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RulesEngine.Rules;
using System.Configuration;
using InterfaceDemo.RulesEngine;
using RulesEngine.Model;

namespace RulesEngine
{
    public static class RuleEngineFactory<T> where T: class
    {
        public static IRuleEngine<T> GetEngine()
        {
            IRuleEngine<T> ruleEngine;

            string configurationString = ConfigurationManager.AppSettings["RuleEngineType"];

            if (configurationString.ToLower() == "xmlruleengine")
            {
                ruleEngine = new XMLRuleEngine<T>();
            }
            else
            {
                ruleEngine = new DefaultRuleEngine<T>();
            }

            return ruleEngine;

            /*
            var t = typeof(T);
            var ruleEngineTypeAttr = t.GetCustomAttributes(typeof(RuleEngineTypeAttribute), true);

            if (ruleEngineTypeAttr != null)
            {
                var ruleType = Activator.CreateInstance((ruleEngineTypeAttr[0] as RuleEngineTypeAttribute).RuleType);
                return ruleType as IRuleEngine<T>;
            }

            return new DefaultRuleEngine<T>();
            */
        }
    }
}
