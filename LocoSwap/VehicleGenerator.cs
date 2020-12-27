using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;

namespace LocoSwap
{
    class VehicleGenerator
    {
        private static XNamespace Namespace = "http://www.kuju.com/TnT/2003/Delta";
        public static Tuple<XElement, ScenarioVehicle> GenerateVehicle(XElement prevElem, AvailableVehicle vehicle)
        {
            var retVehicle = new ScenarioVehicle();
            retVehicle.CopyFrom(vehicle);

            XDocument doc;

            XmlNamespaceManager mgr = new XmlNamespaceManager(new NameTable());
            mgr.AddNamespace("d", "http://www.kuju.com/TnT/2003/Delta");
            XmlParserContext ctx = new XmlParserContext(null, mgr, null, XmlSpace.Default);
            using (XmlReader reader = XmlReader.Create(new StringReader(VehicleTemplates.GetXml(vehicle.Type)), null, ctx))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);
                doc = XDocument.Parse(xmlDoc.OuterXml);
            }

            var cOwnedEntity = doc.Root;

            XElement typeSpecificElement;

            switch (vehicle.Type)
            {
                case VehicleType.Engine:
                    typeSpecificElement = cOwnedEntity.Element("Component").Element("cEngine");
                    break;
                case VehicleType.Wagon:
                    typeSpecificElement = cOwnedEntity.Element("Component").Element("cWagon");
                    break;
                case VehicleType.Tender:
                    typeSpecificElement = cOwnedEntity.Element("Component").Element("cTender");
                    break;
                default:
                    throw new Exception("Unknown vehicle type!");
            }

            typeSpecificElement.Element("UniqueNumber").SetValue(retVehicle.Number);

            var followers = typeSpecificElement.Element("Followers");
            var prevFollowers = prevElem.Element("Component").Descendants("Followers").First().Elements("Network-cTrackFollower");
            foreach (var cTrackFollower in prevFollowers)
            {
                var newFollower = new XElement(cTrackFollower);
                followers.Add(newFollower);
            }

            XElement cEntityContainer = cOwnedEntity
                .Element("Component").Element("cEntityContainer").Element("StaticChildrenMatrix");
            for (var i = 0; i < vehicle.EntityCount; i++)
            {
                var newNode = Utilities.GenerateEntityContainerItem();
                cEntityContainer.Add(newNode);
            }

            XElement cCargoComponent = cOwnedEntity
                .Element("Component").Element("cCargoComponent").Element("InitialLevel");
            for (var i = 0; i < vehicle.CargoCount; i++)
            {
                var newNode = Utilities.GenerateCargoComponentItem(
                    vehicle.CargoComponents[i].Item1,
                    vehicle.CargoComponents[i].Item2);
                cCargoComponent.Add(newNode);
            }

            var cPosOri = new XElement(prevElem.Element("Component").Element("cPosOri"));
            cOwnedEntity.Element("Component").Add(cPosOri);

            cOwnedEntity.Element("Name").SetValue(vehicle.Name);

            var idElements = cOwnedEntity
                .DescendantsAndSelf()
                .Where(elem => elem.Attribute(Namespace + "id") != null);

            Random idRandom = new Random();
            foreach (var elem in idElements)
            {
                var id = idRandom.Next(100000000, 999999999);
                elem.SetAttributeValue(Namespace + "id", id);
            }

            var entityId = cOwnedEntity.Element("EntityID");
            entityId.Add(Utilities.GenerateCGUID());

            XElement cAbsoluteBlueprintID = cOwnedEntity
                .Element("BlueprintID").Element("iBlueprintLibrary-cAbsoluteBlueprintID");
            cAbsoluteBlueprintID
                .Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Provider")
                .SetValue(vehicle.Provider);
            cAbsoluteBlueprintID
                .Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Product")
                .SetValue(vehicle.Product);
            cAbsoluteBlueprintID.Element("BlueprintID").SetValue(vehicle.BlueprintId);

            if (vehicle.IsReskin)
            {
                cAbsoluteBlueprintID = cOwnedEntity
                    .Element("ReskinBlueprintID").Element("iBlueprintLibrary-cAbsoluteBlueprintID");
                cAbsoluteBlueprintID
                    .Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Provider")
                    .SetValue(vehicle.ReskinProvider);
                cAbsoluteBlueprintID
                    .Element("BlueprintSetID").Element("iBlueprintLibrary-cBlueprintSetID").Element("Product")
                    .SetValue(vehicle.ReskinProduct);
                cAbsoluteBlueprintID.Element("BlueprintID").SetValue(vehicle.ReskinBlueprintId);
            }

            return new Tuple<XElement, ScenarioVehicle>(cOwnedEntity, retVehicle);
        }
    }
}
