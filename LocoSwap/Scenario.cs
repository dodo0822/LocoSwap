﻿using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LocoSwap
{
    public class Scenario
    {
        private XDocument ScenarioProperties;
        private XDocument ScenarioXml;
        private XNamespace Namespace = "http://www.kuju.com/TnT/2003/Delta";
        public string RouteId;
        public string Id { get; set; }
        public string Name { get; set; } = "Name not available";
        public string ScenarioDirectory
        {
            get
            {
                return GetScenarioDirectory(RouteId, Id);
            }
        }

        public Scenario()
        {
            RouteId = "";
            Id = "";
            Name = "";
        }

        public Scenario(string routeId, string id)
        {
            Load(routeId, id);
        }

        public void Load(string routeId, string id)
        {
            RouteId = routeId;
            Id = id;

            try
            {
                ScenarioProperties = XmlDocumentLoader.Load(Path.Combine(ScenarioDirectory, "ScenarioProperties.xml"));
                XElement displayName = ScenarioProperties.XPathSelectElement("/cScenarioProperties/DisplayName/Localisation-cUserLocalisedString");
                Name = Utilities.DetermineDisplayName(displayName);
            }
            catch (Exception e)
            {
                Log.Warning("Exception caught when trying to load ScenarioProperties.xml: {0}", e);
                throw new Exception("Malformed ScenarioProperties.xml file!");
            }
        }

        public static string GetScenarioDirectory(string routeId, string id)
        {
            return Path.Combine(GetScenariosDirectory(routeId), id);
        }
        public static string GetScenariosDirectory(string routeId)
        {
            return Path.Combine(Route.GetRouteDirectory(routeId), "Scenarios");
        }

        public static string[] ListAllScenarios(string routeId)
        {
            var routeDirectory = Route.GetRouteDirectory(routeId);
            if (!Directory.Exists(routeDirectory) || !Directory.Exists(GetScenariosDirectory(routeId)))
            {
                return new string[] { };
            }
            List<string> ret = new List<string>();
            var scenarioDirectories = Directory.GetDirectories(GetScenariosDirectory(routeId));
            foreach (var directory in scenarioDirectories)
            {
                string id = new DirectoryInfo(directory).Name;
                string xmlPath = Path.Combine(directory, "ScenarioProperties.xml");
                string binPath = Path.Combine(directory, "Scenario.bin");
                if (!File.Exists(xmlPath) || !File.Exists(binPath)) continue;
                ret.Add(id);
            }
            return ret.ToArray();
        }

        public void ReadScenario(IProgress<int> progress = null)
        {
            progress?.Report(10);

            ScenarioXml = TsSerializer.Load(Path.Combine(ScenarioDirectory, "Scenario.bin"));

            progress?.Report(100);
        }

        public List<Consist> GetConsists(IProgress<int> progress = null)
        {
            progress?.Report(0);
            List<Consist> ret = new List<Consist>();
            IEnumerable<XElement> consists = ScenarioXml.Root.Descendants("cConsist");
            foreach (var consistRow in consists.Select((value, i) => (value, i)))
            {
                XElement consist = consistRow.value;
                int consistIdx = consistRow.i;
                Consist consistObj = new Consist();

                XElement driver = consist.Element("Driver").Element("cDriver");
                string name = "Loose Consist";
                if (driver != null)
                {
                    IEnumerable<XElement> names = driver.Element("ServiceName").Element("Localisation-cUserLocalisedString").Elements();
                    foreach (XElement nameElement in names)
                    {
                        if (nameElement.Name == "Key" || nameElement.Name == "Other" || nameElement.Value == "") continue;
                        name = nameElement.Value;
                        break;
                    }
                    XElement playerDriver = driver.Element("PlayerDriver");
                    if (playerDriver.Value == "1")
                    {
                        consistObj.IsPlayerConsist = true;
                    }
                }

                IEnumerable<XElement> vehicles = consist.Element("RailVehicles").Descendants("cOwnedEntity");
                foreach (var vehicleRow in vehicles.Select((value, i) => (value, i)))
                {
                    int vehicleIdx = vehicleRow.i;

                    // Fill in basic info
                    XElement vehicle = vehicleRow.value;
                    XElement blueprintID = vehicle.Element("BlueprintID").Element("iBlueprintLibrary-cAbsoluteBlueprintID");
                    string provider = (string)blueprintID.Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Provider").Value;
                    string product = (string)blueprintID.Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Product").Value;
                    string path = (string)blueprintID.Element("BlueprintID").Value;
                    string vehicleName = vehicle.Element("Name").Value;
                    VehicleType type;
                    if (vehicle.Descendants("cEngine").Count() > 0)
                        type = VehicleType.Engine;
                    else
                        type = VehicleType.Wagon;
                    string number = "";
                    XElement uniqueNumber = vehicle.Descendants("UniqueNumber").FirstOrDefault();
                    if (uniqueNumber != null)
                    {
                        number = uniqueNumber.Value;
                    }
                    bool flipped = false;
                    XElement flippedElement = vehicle.Descendants("Flipped").FirstOrDefault();
                    if (flippedElement != null)
                    {
                        flipped = flippedElement.Value == "1";
                    }

                    ScenarioVehicle v = new ScenarioVehicle(vehicleIdx, provider, product, path, vehicleName, number, flipped);
                    v.Type = type;

                    // Determine if it is a reskin
                    XElement reskinBlueprintIdElement = vehicle.Element("ReskinBlueprintID");
                    try
                    {
                        var reskinProvider = reskinBlueprintIdElement.Descendants("Provider").First().Value;
                        if (reskinProvider != "")
                        {
                            var reskinProduct = reskinBlueprintIdElement.Descendants("Product").First().Value;
                            var reskinBlueprintId = reskinBlueprintIdElement.Descendants("BlueprintID").First().Value;
                            v.IsReskin = true;
                            v.ReskinProvider = reskinProvider;
                            v.ReskinProduct = reskinProduct;
                            v.ReskinBlueprintId = reskinBlueprintId;
                            Log.Debug("{0} is a reskin at {1}\\{2}\\{3}", v.Name, reskinProvider, reskinProduct, reskinBlueprintId);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Debug("Exception caught when determining whether a ScenarioVehicle is reskin\n{0}", e);
                    }

                    consistObj.Vehicles.Add(v);
                }
                consistObj.Idx = consistIdx;
                consistObj.Name = name;
                ret.Add(consistObj);
            }

            foreach (var row in ret.Select((value, i) => (value, i)))
            {
                Consist consist = row.value;

                foreach (var vehicle in consist.Vehicles)
                {
                    VehicleAvailibilityResult abilility = VehicleAvailibility.IsVehicleAvailable(vehicle);
                    if (abilility.Available)
                    {
                        if (vehicle.DisplayName != "") continue;
                        vehicle.DisplayName = VehicleAvailibility.GetVehicleDisplayName(vehicle);
                        continue;
                    }
                    vehicle.Exists = VehicleExistance.Missing;
                    consist.IsComplete = ConsistVehicleExistance.Missing;
                }
                progress?.Report((int)Math.Ceiling((float)row.i / ret.Count * 100));
            }

            return ret;
        }

        private void CreateBlueprintSetPreLoad(string provider, string product)
        {
            XElement rBlueprintSetPreLoad = ScenarioProperties.Root.Element("RBlueprintSetPreLoad");
            IEnumerable<XElement> entriesFound = from item in rBlueprintSetPreLoad.Elements("iBlueprintLibrary-cBlueprintSetID")
                                                 where item.Element("Provider").Value.ToString() == provider &&
                                                       item.Element("Product").Value.ToString() == product
                                                 select item;
            if (entriesFound.FirstOrDefault() != null) return;

            XElement entry = new XElement("iBlueprintLibrary-cBlueprintSetID");
            Random idRandom = new Random();

            int id;
            do
            {
                id = idRandom.Next(100000000, 999999999);
            } while (ScenarioProperties.Descendants().Where(elem => (elem.Attribute(Namespace + "id") != null && elem.Attribute(Namespace + "id").Value == id.ToString())).Any());

            entry.SetAttributeValue(Namespace + "id", id.ToString());
            XElement providerElement = new XElement("Provider");
            providerElement.SetAttributeValue(Namespace + "type", "cDeltaString");
            providerElement.SetValue(provider);
            XElement productElement = new XElement("Product");
            productElement.SetAttributeValue(Namespace + "type", "cDeltaString");
            productElement.SetValue(product);
            entry.Add(providerElement, productElement);

            rBlueprintSetPreLoad.Add(entry);

            return;
        }

        public void ReplaceVehicle(int consistIdx, int vehicleIdx, AvailableVehicle newVehicle)
        {
            // Locate the consist and vehicle
            XElement consist = ScenarioXml.Root.Descendants("cConsist").Skip(consistIdx).FirstOrDefault();
            if (consist == null)
            {
                throw new Exception("Consist not found");
            }
            XElement vehicle = consist.Element("RailVehicles").Descendants("cOwnedEntity").Skip(vehicleIdx).FirstOrDefault();
            if (vehicle == null)
            {
                throw new Exception("Vehicle not found");
            }

            // Update blueprint ID in Scenario.bin
            XElement blueprintID = vehicle.Element("BlueprintID").Element("iBlueprintLibrary-cAbsoluteBlueprintID");
            var origBlueprintID = blueprintID.Element("BlueprintID").Value;
            blueprintID.Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Provider").SetValue(newVehicle.Provider);
            blueprintID.Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Product").SetValue(newVehicle.Product);
            blueprintID.Element("BlueprintID").SetValue(newVehicle.BlueprintId);
            vehicle.Element("Name").SetValue(newVehicle.Name);

            // Update reskin info
            XElement reskinBlueprintID = vehicle.Element("ReskinBlueprintID").Element("iBlueprintLibrary-cAbsoluteBlueprintID");
            if (newVehicle.IsReskin)
            {
                Log.Debug("ReplaceVehicle: Reskin set to {0}", newVehicle.ReskinXmlPath);
                reskinBlueprintID.Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Provider").SetValue(newVehicle.ReskinProvider);
                reskinBlueprintID.Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Product").SetValue(newVehicle.ReskinProduct);
                reskinBlueprintID.Element("BlueprintID").SetValue(newVehicle.ReskinBlueprintId);
            }
            else
            {
                // Remove reskin info
                Log.Debug("ReplaceVehicle: Remove reskin info");
                reskinBlueprintID.Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Provider").SetValue("");
                reskinBlueprintID.Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Product").SetValue("");
                reskinBlueprintID.Element("BlueprintID").SetValue("");
            }

            // Update engine and wagon dependent parameters
            XElement cElement = vehicle.Descendants().Where(element => element.Name == "cWagon" || element.Name == "cEngine" || element.Name == "cTender").FirstOrDefault();
            if (cElement == null)
            {
                throw new Exception("Cannot find cWagon or cEngine element!");
            }

            if (cElement.Name == "cEngine" && newVehicle.Type != VehicleType.Engine)
            {
                // We should remove additional nodes for engines
                cElement.Elements()
                    .Where(element => element.Name == "DisabledEngine" || element.Name == "AWSTimer" || element.Name == "AWSExpired" || element.Name == "TPWSDistance")
                    .Remove();

                vehicle.Element("Component").Element("cEngineSimContainer").Remove();
            }
            if (cElement.Name == "cTender" && newVehicle.Type != VehicleType.Tender)
            {
                // We should remove additional nodes for tenders
                cElement.Elements()
                    .Where(element => element.Name == "CoalLevel" || element.Name == "WaterLevel")
                    .Remove();
            }

            if (newVehicle.Type == VehicleType.Wagon)
            {
                cElement.Name = "cWagon";
            }
            else if (newVehicle.Type == VehicleType.Engine && cElement.Name != "cEngine")
            {
                // We should create additional nodes for engines
                cElement.Name = "cEngine";
                Log.Debug("Creating additional nodes for replacement {0}", newVehicle.DisplayName);
                XElement disabledEngine = new XElement("DisabledEngine");
                disabledEngine.SetAttributeValue(Namespace + "type", "bool");
                disabledEngine.SetValue("0");

                XElement awsTimer = new XElement("AWSTimer");
                awsTimer.SetAttributeValue(Namespace + "type", "sFloat32");
                awsTimer.SetAttributeValue(Namespace + "alt_encoding", "0000000000000000");
                awsTimer.SetAttributeValue(Namespace + "precision", "string");
                awsTimer.SetValue("0");

                XElement awsExpired = new XElement("AWSExpired");
                awsExpired.SetAttributeValue(Namespace + "type", "bool");
                awsExpired.SetValue("0");

                XElement tpwsDistance = new XElement("TPWSDistance");
                tpwsDistance.SetAttributeValue(Namespace + "type", "sFloat32");
                tpwsDistance.SetAttributeValue(Namespace + "alt_encoding", "0000000000000000");
                tpwsDistance.SetAttributeValue(Namespace + "precision", "string");
                tpwsDistance.SetValue("0");

                cElement.Add(disabledEngine, awsTimer, awsExpired, tpwsDistance);

                Random idRandom = new Random();

                int id;
                do
                {
                    id = idRandom.Next(100000000, 999999999);
                } while (ScenarioXml.Descendants().Where(elem => (elem.Attribute(Namespace + "id") != null && elem.Attribute(Namespace + "id").Value == id.ToString())).Any());
                XElement cEngineSimContainer = new XElement("cEngineSimContainer");
                cEngineSimContainer.SetAttributeValue(Namespace + "id", id.ToString());
                vehicle.Element("Component").Add(cEngineSimContainer);
            }
            else if (newVehicle.Type == VehicleType.Tender && cElement.Name != "cTender")
            {
                cElement.Name = "cTender";
                Log.Debug("Creating additional tender nodes for replacement {0}", newVehicle.DisplayName);

                XElement coalLevel = new XElement("CoalLevel");
                coalLevel.SetAttributeValue(Namespace + "type", "sFloat32");
                coalLevel.SetAttributeValue(Namespace + "alt_encoding", "0000000000000000");
                coalLevel.SetAttributeValue(Namespace + "precision", "string");
                coalLevel.SetValue("0");

                XElement waterLevel = new XElement("WaterLevel");
                waterLevel.SetAttributeValue(Namespace + "type", "sFloat32");
                waterLevel.SetAttributeValue(Namespace + "alt_encoding", "0000000000000000");
                waterLevel.SetAttributeValue(Namespace + "precision", "string");
                waterLevel.SetValue("0");

                cElement.Add(coalLevel, waterLevel);
            }

            // Cargo component count matching
            XElement cCargoComponent = vehicle.Element("Component").Element("cCargoComponent").Element("InitialLevel");
            int cargoCount = cCargoComponent.Elements().Count();
            if (newVehicle.CargoCount > cargoCount)
            {
                Log.Debug("Need to create cargo initial level holders {0} -> {1}", cargoCount, newVehicle.CargoCount);
                for (int i = cargoCount; i < newVehicle.CargoCount; ++i)
                {
                    var newNode = Utilities.GenerateCargoComponentItem(
                        newVehicle.CargoComponents[i].Item1,
                        newVehicle.CargoComponents[i].Item2);
                    cCargoComponent.Add(newNode);
                }
            }
            else if (newVehicle.CargoCount < cargoCount)
            {
                Log.Debug("Need to remove cargo initial level holders {0} -> {1}", cargoCount, newVehicle.CargoCount);
                cCargoComponent.Elements().Take(cargoCount - newVehicle.CargoCount).Remove();
            }

            // Entity container count matching
            XElement cEntityContainer = vehicle.Element("Component").Element("cEntityContainer").Element("StaticChildrenMatrix");
            int entityCount = cEntityContainer.Elements().Count();
            if (newVehicle.EntityCount > entityCount)
            {
                Log.Debug("Need to add entities {0} -> {1}", entityCount, newVehicle.EntityCount);
                for (int i = entityCount; i < newVehicle.EntityCount; ++i)
                {
                    var newNode = Utilities.GenerateEntityContainerItem();
                    cEntityContainer.Add(newNode);
                }
            }
            else if (newVehicle.EntityCount < entityCount)
            {
                Log.Debug("Need to remove entities {0} -> {1}", entityCount, newVehicle.EntityCount);
                cEntityContainer.Elements().Take(entityCount - newVehicle.EntityCount).Remove();
            }

            // If the consist is driven by the player, loco name in ScenarioProperties.xml should be updated
            if (consist.Element("Driver").Element("cDriver") != null)
            {
                XElement cDriver = consist.Element("Driver").Element("cDriver");
                if (cDriver.Element("PlayerDriver") != null)
                {
                    if (cDriver.Element("PlayerDriver").Value == "1")
                    {
                        Log.Debug("Train is driven by player; checking whether the loco is swapped");
                        var key = cDriver.Element("ServiceName").Descendants("Key").First().Value;
                        XElement sDriverFrontEndDetails = ScenarioProperties.Root.Element("FrontEndDriverList")
                            .Elements("sDriverFrontEndDetails").Where(element => element.Element("ServiceName").Element("Localisation-cUserLocalisedString").Element("Key").Value == key).FirstOrDefault();
                        if (sDriverFrontEndDetails == null)
                        {
                            Log.Debug("Could not find sDriverFrontEndDetails with key {0}!", key);
                        }
                        else
                        {
                            var consistLocoBlueprintId = sDriverFrontEndDetails.Element("LocoBP").Descendants("BlueprintID").First().Value;
                            if (consistLocoBlueprintId == origBlueprintID)
                            {
                                Log.Debug("Update new sDriverFrontEndDetails to Blueprint ID {0}", newVehicle.BlueprintId);
                                XElement destLocalisedString = sDriverFrontEndDetails.Element("LocoName").Element("Localisation-cUserLocalisedString");
                                XElement origLocalisedString = newVehicle.NameLocalisedString;
                                Utilities.CopyUserLocalisedString(destLocalisedString, origLocalisedString);
                                sDriverFrontEndDetails.Element("LocoBP").Descendants("Provider").First().Value = newVehicle.Provider;
                                sDriverFrontEndDetails.Element("LocoBP").Descendants("Product").First().Value = newVehicle.Product;
                                sDriverFrontEndDetails.Element("LocoBP").Descendants("BlueprintID").First().Value = newVehicle.BlueprintId;
                                sDriverFrontEndDetails.Element("LocoAuthor").Value = newVehicle.Provider;
                                string newVehicleXmlPath = newVehicle.XmlPath;
                                sDriverFrontEndDetails.Element("FilePath").Value = newVehicleXmlPath.Substring(0, newVehicleXmlPath.LastIndexOf('\\'));
                            }
                        }
                    }
                }
            }

            CreateBlueprintSetPreLoad(newVehicle.Provider, newVehicle.Product);
            if (newVehicle.IsReskin)
            {
                CreateBlueprintSetPreLoad(newVehicle.ReskinProvider, newVehicle.ReskinProduct);
            }
        }

        public void ChangeVehicleNumber(int consistIdx, int vehicleIdx, string newNumber)
        {
            XElement consist = ScenarioXml.Root.Descendants("cConsist").Skip(consistIdx).FirstOrDefault();
            if (consist == null)
            {
                throw new Exception("Consist not found");
            }
            XElement vehicle = consist.Element("RailVehicles").Descendants("cOwnedEntity").Skip(vehicleIdx).FirstOrDefault();
            if (vehicle == null)
            {
                throw new Exception("Vehicle not found");
            }
            XElement uniqueNumber = vehicle.Descendants("UniqueNumber").FirstOrDefault();

            string originalNumber = uniqueNumber.Value;
            Log.Debug("Changing vehicle number {0} to {1}", originalNumber, newNumber);

            // Check InitialRV lists
            var cDriver = consist.Element("Driver").Element("cDriver");
            if (cDriver != null)
            {
                var initialRV = cDriver.Element("InitialRV");
                var row = initialRV.Elements("e").Skip(vehicleIdx).FirstOrDefault();
                if (row != null)
                {
                    if (row.Value == originalNumber)
                    {
                        Log.Debug("InitialRV number matched, going to update");
                        row.Value = newNumber;
                    }
                    else
                    {
                        Log.Debug("InitialRV number did not match, vehicle is {0}, initialRV is {1}, skipping update.", originalNumber, row.Value);
                    }
                }
            }

            // Check number in instructions
            var cConsistOperations = ScenarioXml.Root.Descendants("cConsistOperations");
            var operationTargetNumbers = from item in cConsistOperations.Elements("DeltaTarget").Elements("cDriverInstructionTarget").Elements("RailVehicleNumber").Elements("e")
                                         where item.Value == originalNumber
                                         select item;
            Log.Debug("{0} matching instructions found", operationTargetNumbers.Count());
            foreach (XElement e in operationTargetNumbers)
            {
                e.Value = newNumber;
            }

            uniqueNumber.SetValue(newNumber);
        }

        public void ChangeVehicleFlipped(int consistIdx, int vehicleIdx, bool flipped)
        {
            XElement consist = ScenarioXml.Root.Descendants("cConsist").Skip(consistIdx).FirstOrDefault();
            if (consist == null)
            {
                throw new Exception("Consist not found");
            }
            XElement vehicle = consist.Element("RailVehicles").Descendants("cOwnedEntity").Skip(vehicleIdx).FirstOrDefault();
            if (vehicle == null)
            {
                throw new Exception("Vehicle not found");
            }
            XElement flippedElement = vehicle.Descendants("Flipped").FirstOrDefault();
            if (flippedElement == null)
            {
                Log.Debug("Consist {0}, Vehicle {1} does not have Flipped property!", consistIdx, vehicleIdx);
                return;
            }
            flippedElement.Value = flipped ? "1" : "0";
            return;
        }

        public ScenarioVehicle InsertVehicle(int consistIdx, int insertPos, AvailableVehicle vehicle)
        {
            XElement consist = ScenarioXml.Root.Descendants("cConsist").Skip(consistIdx).FirstOrDefault();
            if (consist == null)
            {
                throw new Exception("Consist not found");
            }
            XElement vehicles = consist.Element("RailVehicles");

            bool atEnd = insertPos >= vehicles.Elements("cOwnedEntity").Count();

            var insert = VehicleGenerator.GenerateVehicle(
                atEnd ? vehicles.Elements("cOwnedEntity").Last() : vehicles.Elements("cOwnedEntity").ElementAt(insertPos),
                vehicle);
            var elem = insert.Item1;
            elem.Attribute(XNamespace.Xmlns + "d").Remove();

            if (atEnd)
            {
                vehicles.Add(elem);
            }
            else
            {
                vehicles.Elements("cOwnedEntity").ElementAt(insertPos).AddBeforeSelf(elem);
            }

            // Check InitialRV lists
            var cDriver = consist.Element("Driver").Element("cDriver");
            if (cDriver != null)
            {
                var e = new XElement("e");
                e.SetAttributeValue(Namespace + "type", "cDeltaString");
                e.SetValue(insert.Item2.Number);

                var initialRV = cDriver.Element("InitialRV");
                if (atEnd)
                {
                    initialRV.Add(e);
                }
                else
                {
                    var row = initialRV.Elements("e").ElementAt(insertPos);
                    if (row != null)
                    {
                        row.AddBeforeSelf(e);
                    }
                }
            }

            CreateBlueprintSetPreLoad(vehicle.Provider, vehicle.Product);

            return insert.Item2;
        }

        public void RemoveVehicle(int consistIdx, int vehicleIdx)
        {
            XElement consist = ScenarioXml.Root.Descendants("cConsist").Skip(consistIdx).FirstOrDefault();
            if (consist == null)
            {
                throw new Exception("Consist not found");
            }
            XElement vehicles = consist.Element("RailVehicles");
            if (vehicles.Elements("cOwnedEntity").Count() <= vehicleIdx)
            {
                throw new Exception("Vehicle index out of range");
            }
            var elem = vehicles.Elements("cOwnedEntity").ElementAt(vehicleIdx);
            var uniqueNumber = elem.Descendants("UniqueNumber").FirstOrDefault();

            string originalNumber = uniqueNumber.Value;
            // Check InitialRV lists
            var cDriver = consist.Element("Driver").Element("cDriver");
            if (cDriver != null)
            {
                var initialRV = cDriver.Element("InitialRV");
                var row = initialRV.Elements("e").Skip(vehicleIdx).FirstOrDefault();
                if (row != null)
                {
                    if (row.Value == originalNumber)
                    {
                        Log.Debug("InitialRV number matched, going to remove");
                        row.Remove();
                    }
                    else
                    {
                        Log.Debug("InitialRV number did not match, vehicle is {0}, initialRV is {1}, skipping removal.", originalNumber, row.Value);
                    }
                }
            }

            // Check number in instructions
            var cConsistOperations = ScenarioXml.Root.Descendants("cConsistOperations");
            var operationTargetNumbers = from item in cConsistOperations.Elements("DeltaTarget").Elements("cDriverInstructionTarget").Elements("RailVehicleNumber").Elements("e")
                                         where item.Value == originalNumber
                                         select item;
            Log.Debug("{0} matching instructions found", operationTargetNumbers.Count());
            foreach (XElement e in operationTargetNumbers)
            {
                e.Remove();
            }

            elem.Remove();

            return;
        }

        public void Save()
        {
            string propertiesXmlPath = Path.Combine(Utilities.GetTempDir(), "ScenarioProperties.xml");
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.IndentChars = "\t";
            xmlWriterSettings.Encoding = new UTF8Encoding(false);

            FileStream stream = new FileStream(propertiesXmlPath, FileMode.Create);

            // Append a suffix to scenario DisplayName
            IEnumerable<XElement> displayNameLanguageNodes = ScenarioProperties.XPathSelectElements("/cScenarioProperties/DisplayName/Localisation-cUserLocalisedString/*");
            foreach (XElement displayNameLanguageNode in displayNameLanguageNodes)
            {
                if (displayNameLanguageNode.Name != "Key" && displayNameLanguageNode.Value != "" && !displayNameLanguageNode.Value.EndsWith("[LoSw]"))
                {
                    displayNameLanguageNode.SetValue(displayNameLanguageNode.Value + " [LoSw]");
                }
            }

            using (XmlWriter writer = XmlWriter.Create(stream, xmlWriterSettings))
            {
                ScenarioProperties.Save(writer);
            }
            stream.Flush();
            stream.Close();

            TsSerializer.Save(ScenarioXml, Path.Combine(Utilities.GetTempDir(), "Scenario.bin"));

            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var scenarioFileName = Path.Combine(ScenarioDirectory, "Scenario.bin");
            var scenarioBackupFileName = Path.Combine(ScenarioDirectory, string.Format("ScenarioBackup-{0}.bin", timestamp));
            var scenarioPropertiesFileName = Path.Combine(ScenarioDirectory, "ScenarioProperties.xml");
            var scenarioPropertiesBackupFileName = Path.Combine(ScenarioDirectory, string.Format("ScenarioPropertiesBackup-{0}.xml", timestamp));

            File.Copy(scenarioFileName, scenarioBackupFileName);
            File.Copy(scenarioPropertiesFileName, scenarioPropertiesBackupFileName);

            File.Copy(Path.Combine(Utilities.GetTempDir(), "Scenario.bin"), scenarioFileName, true);
            File.Copy(propertiesXmlPath, scenarioPropertiesFileName, true);
        }
    }

}
