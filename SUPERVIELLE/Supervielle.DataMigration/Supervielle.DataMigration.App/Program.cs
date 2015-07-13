using Newtonsoft.Json;
using Supervielle.DataMigration.BusinessLogic;
using Supervielle.DataMigration.BusinessLogic.Helpers;
using Supervielle.DataMigration.BusinessLogic.Intefaces;
using Supervielle.DataMigration.Domain;
using Supervielle.DataMigration.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataMigration.App
{
    class Program
    {
        private static IOneToOneMigrationService oneToOneMigrationService;
        private static IEntityMigrationService entityMigrationService;
        private static IProductMigrationService productMigrationService;

        static int Main(string[] args)
        {
            try
            {
                InitializeDependencies();
                var arguments = ValidateArguments(args);

                Run(arguments);

                return Properties.Settings.Default.ExitCodeSuccess;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Properties.Settings.Default.ExitCodeError;
            }
        }

        private static void Run(IEnumerable<Argument> arguments)
        {
            foreach (var argument in arguments)
            {
                ProcessArgument(argument);
            }
        }

        private static void ProcessArgument(Argument argument)
        {
            switch (argument.ArgumentType)
            {
                case ArgumentType.OneToOne:
                    oneToOneMigrationService.Migrate(argument.AvailableValues);
                    break;
                case ArgumentType.Entities:
                    entityMigrationService.Migrate();
                    break;
                case ArgumentType.Products:
                    productMigrationService.Migrate(argument.AvailableValues);
                    break;
                default:
                    break;
            }
        }

        private static void InitializeDependencies()
        {
            //oneToOneMigrationService = new OneToOneMigrationService();
            entityMigrationService = new EntityMigrationService();
            productMigrationService = new ProductMigrationService();
        }

        private static IEnumerable<Argument> ValidateArguments(string[] args)
        {
            var possibleArguments = JsonConvert.DeserializeObject<List<Argument>>(File.ReadAllText(Properties.Settings.Default.ArgumentMappingFilePath));
            var currentArguments = new List<Argument>();

            // Iterate the input arguments;
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                // If the argument is an available option of the configured Arguments
                if (IsArgumentOption(possibleArguments, arg))
                {
                    var possibleArgument = possibleArguments.First(x => x.ArgumentOption == arg);
                    var currentArgument = new Argument(possibleArgument.ArgumentOption, possibleArgument.ArgumentType);

                    // If the argument option is not already in the current execution Arguments
                    if (!IsArgumentOption(currentArguments, arg))
                    {
                        // Iterate the available Values of the current Argument
                        for (int j = i + 1; j < args.Length; j++)
                        {
                            var valueArg = args[j];
                            
                            // if the current argument is an option
                            if (IsArgumentOption(possibleArguments, valueArg))
                            {
                                break;
                            }
                            else
                            {
                                // Is the Argument is an available value
                                if (IsArgumentAvailableValue(possibleArgument, valueArg))
                                {
                                    var availableValue = possibleArgument.AvailableValues.First(x => x.Name == valueArg);
                                    currentArgument.AvailableValues.Add(availableValue);
                                }
                                else
                                {
                                    throw new Exception("The value '" + arg + "' is not available in for the option '" + valueArg + "'.");
                                }
                            }

                            i = j;
                        }
                    }
                    else
                    {
                        throw new Exception("The option '" + arg + "' appears more than once.");
                    }

                    currentArguments.Add(currentArgument);
                }
                else
                {
                    throw new Exception("The option '" + arg + "' is not available.");
                }
            }

            return currentArguments;
        }

        private static bool IsArgumentAvailableValue(Argument argument, string valueArg)
        {
            return argument.AvailableValues.Any(x => x.Name == valueArg);
        }

        private static bool IsArgumentOption(List<Argument> possibleArguments, string arg)
        {
            return possibleArguments.Any(x => x.ArgumentOption == arg);
        }
    }
}
