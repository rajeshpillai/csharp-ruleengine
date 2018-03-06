using RulesEngine;
using RulesEngine.Rules;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace InterfaceDemo.RulesEngine
{
    public class XMLRuleEngine<T> : RuleEngineBase<T>, IRuleEngine<T>
    {
        public List<BrokenRule> Validate(T value)
        {
            var results = new List<BrokenRule>();
            var props = value.GetType().GetProperties();

            foreach (var prop in props)
            {
                var rules = Rules[prop.Name];

                foreach (var rule in rules)
                {
                    var ruleAttribute = rule as ValidationAttribute;
                    var ruleResult = ruleAttribute.Validate(prop.GetValue(value), new ValidationContext { SourceObject = value });
                    if (ruleResult.IsBroken)
                    {
                        results.Add(ruleResult);
                    }
                }
            }
            return results;

        }
        public override void BuildRuleSet()
        {
            System.Type value = typeof(T);

            var props = value.GetProperties();
            var xdoc = XDocument.Load(Environment.CurrentDirectory + @"\RuleConfig\" + value.Name + ".xml");

            foreach (var prop in props)
            {
                var rulesAtts = new List<ValidationAttribute>();
                foreach (var itm in xdoc.Descendants("Property"))
                {
                    if (itm.Attribute("Name").Value == prop.Name)
                    {
                        foreach (var item in itm.Descendants("Validator"))
                        {

                            var validationType = item.Attribute("Type").Value;
                            if (validationType == "MaxLenFieldAttribute")
                            {
                                var errmsg = item.Attribute("ErrorMessage").Value;
                                var max = item.Attribute("Max").Value;
                                rulesAtts.Add(new MaxLenFieldAttribute(prop.Name, errmsg, Convert.ToInt32(max)));
                            }
                            if (validationType == "RequiredFieldAttribute")
                            {
                                var errmsg = item.Attribute("ErrorMessage").Value;
                                rulesAtts.Add(new RequiredFieldAttribute(prop.Name, errmsg));
                            }
                        }
                    }
                }
                // var rulesAtts = prop.GetCustomAttributes(typeof(ValidationAttribute), true);
                var ruleItems = new List<ValidationAttribute>();

                foreach (var rule in rulesAtts)
                {
                    var ruleAttribute = rule as ValidationAttribute;
                    ruleItems.Add(ruleAttribute);
                }

                Rules[prop.Name] = ruleItems;
            }

        }
    }
}
