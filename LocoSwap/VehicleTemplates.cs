using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LocoSwap
{
    class VehicleTemplates
    {
        public const string EngineXml = @"
<cOwnedEntity d:id="""">
	<Component>
		<cEngine d:id="""">
			<PantographInfo d:type=""cDeltaString"">PANTO_UNKNOWN</PantographInfo>
			<PantographIsDirectional d:type=""bool"">0</PantographIsDirectional>
			<LastPantographControlValue d:type=""sFloat32"" d:alt_encoding=""000000000000F03F"" d:precision=""string"">1</LastPantographControlValue>
			<Flipped d:type=""bool"">0</Flipped>
			<UniqueNumber d:type=""cDeltaString""></UniqueNumber>
			<GUID d:type=""cDeltaString""></GUID>
			<Followers>
			</Followers>
			<TotalMass d:type=""sFloat32"" d:alt_encoding=""0000000000405440"" d:precision=""string"">81</TotalMass>
			<Speed d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</Speed>
			<Velocity>
				<cHcRVector4>
					<Element>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
					</Element>
				</cHcRVector4>
			</Velocity>
			<InTunnel d:type=""bool"">0</InTunnel>
			<DisabledEngine d:type=""bool"">0</DisabledEngine>
			<AWSTimer d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</AWSTimer>
			<AWSExpired d:type=""bool"">0</AWSExpired>
			<TPWSDistance d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</TPWSDistance>
		</cEngine>
		<cAnimObjectRender d:id="""">
			<DetailLevel d:type=""sInt32"">10</DetailLevel>
			<Global d:type=""bool"">0</Global>
			<Saved d:type=""bool"">1</Saved>
			<Palette0Index d:type=""sUInt8"">151</Palette0Index>
			<Palette1Index d:type=""sUInt8"">83</Palette1Index>
			<Palette2Index d:type=""sUInt8"">159</Palette2Index>
		</cAnimObjectRender>
		<cEngineSimContainer d:id=""""/>
		<cControlContainer d:id="""">
			<Time d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</Time>
			<FrameTime d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</FrameTime>
			<CabEndWithKey d:type=""cDeltaString"">eAEnd</CabEndWithKey>
		</cControlContainer>
		<cEntityContainer d:id="""">
			<StaticChildrenMatrix>
			</StaticChildrenMatrix>
		</cEntityContainer>
		<cScriptComponent d:id="""">
			<DebugDisplay d:type=""bool"">1</DebugDisplay>
			<StateName d:type=""cDeltaString""></StateName>
		</cScriptComponent>
		<cCargoComponent d:id="""">
			<IsPreLoaded d:type=""cDeltaString"">eFalse</IsPreLoaded>
			<InitialLevel>
			</InitialLevel>
		</cCargoComponent>
	</Component>
	<BlueprintID>
		<iBlueprintLibrary-cAbsoluteBlueprintID>
			<BlueprintSetID>
				<iBlueprintLibrary-cBlueprintSetID>
					<Provider d:type=""cDeltaString""></Provider>
					<Product d:type=""cDeltaString""></Product>
				</iBlueprintLibrary-cBlueprintSetID>
			</BlueprintSetID>
			<BlueprintID d:type=""cDeltaString""></BlueprintID>
		</iBlueprintLibrary-cAbsoluteBlueprintID>
	</BlueprintID>
	<ReskinBlueprintID>
		<iBlueprintLibrary-cAbsoluteBlueprintID>
			<BlueprintSetID>
				<iBlueprintLibrary-cBlueprintSetID>
					<Provider d:type=""cDeltaString""></Provider>
					<Product d:type=""cDeltaString""></Product>
				</iBlueprintLibrary-cBlueprintSetID>
			</BlueprintSetID>
			<BlueprintID d:type=""cDeltaString""></BlueprintID>
		</iBlueprintLibrary-cAbsoluteBlueprintID>
	</ReskinBlueprintID>
	<Name d:type=""cDeltaString""></Name>
	<EntityID>
	</EntityID>
</cOwnedEntity>";

		public static string WagonXml = @"
<cOwnedEntity d:id="""">
	<Component>
		<cWagon d:id="""">
			<PantographInfo d:type=""cDeltaString"">PANTO_UNKNOWN</PantographInfo>
			<PantographIsDirectional d:type=""bool"">0</PantographIsDirectional>
			<LastPantographControlValue d:type=""sFloat32"" d:alt_encoding=""000000000000F03F"" d:precision=""string"">1</LastPantographControlValue>
			<Flipped d:type=""bool"">0</Flipped>
			<UniqueNumber d:type=""cDeltaString""></UniqueNumber>
			<GUID d:type=""cDeltaString""></GUID>
			<Followers>
			</Followers>
			<TotalMass d:type=""sFloat32"" d:alt_encoding=""0000006066662D40"" d:precision=""string"">14.7</TotalMass>
			<Speed d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</Speed>
			<Velocity>
				<cHcRVector4>
					<Element>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
					</Element>
				</cHcRVector4>
			</Velocity>
			<InTunnel d:type=""bool"">0</InTunnel>
		</cWagon>
		<cAnimObjectRender d:id="""">
			<DetailLevel d:type=""sInt32"">10</DetailLevel>
			<Global d:type=""bool"">0</Global>
			<Saved d:type=""bool"">1</Saved>
			<Palette0Index d:type=""sUInt8"">0</Palette0Index>
			<Palette1Index d:type=""sUInt8"">0</Palette1Index>
			<Palette2Index d:type=""sUInt8"">0</Palette2Index>
		</cAnimObjectRender>
		<cControlContainer d:id="""">
			<Time d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</Time>
			<FrameTime d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</FrameTime>
			<CabEndWithKey d:type=""cDeltaString"">eNoEnd</CabEndWithKey>
		</cControlContainer>
		<cCargoComponent d:id="""">
			<IsPreLoaded d:type=""cDeltaString"">eFalse</IsPreLoaded>
			<InitialLevel />
		</cCargoComponent>
		<cEntityContainer d:id="""">
			<StaticChildrenMatrix />
		</cEntityContainer>
		<cScriptComponent d:id="""">
			<DebugDisplay d:type=""bool"">1</DebugDisplay>
			<StateName d:type=""cDeltaString""></StateName>
		</cScriptComponent>
	</Component>
	<BlueprintID>
		<iBlueprintLibrary-cAbsoluteBlueprintID>
			<BlueprintSetID>
				<iBlueprintLibrary-cBlueprintSetID>
					<Provider d:type=""cDeltaString""></Provider>
					<Product d:type=""cDeltaString""></Product>
				</iBlueprintLibrary-cBlueprintSetID>
			</BlueprintSetID>
			<BlueprintID d:type=""cDeltaString""></BlueprintID>
		</iBlueprintLibrary-cAbsoluteBlueprintID>
	</BlueprintID>
	<ReskinBlueprintID>
		<iBlueprintLibrary-cAbsoluteBlueprintID>
			<BlueprintSetID>
				<iBlueprintLibrary-cBlueprintSetID>
					<Provider d:type=""cDeltaString""></Provider>
					<Product d:type=""cDeltaString""></Product>
				</iBlueprintLibrary-cBlueprintSetID>
			</BlueprintSetID>
			<BlueprintID d:type=""cDeltaString""></BlueprintID>
		</iBlueprintLibrary-cAbsoluteBlueprintID>
	</ReskinBlueprintID>
	<Name d:type=""cDeltaString""></Name>
	<EntityID>
	</EntityID>
</cOwnedEntity>";

		public static string TenderXml = @"
<cOwnedEntity d:id="""">
	<Component>
		<cTender d:id="""">
			<PantographInfo d:type=""cDeltaString"">PANTO_UNKNOWN</PantographInfo>
			<PantographIsDirectional d:type=""bool"">0</PantographIsDirectional>
			<LastPantographControlValue d:type=""sFloat32"" d:alt_encoding=""000000000000F03F"" d:precision=""string"">1</LastPantographControlValue>
			<Flipped d:type=""bool"">0</Flipped>
			<UniqueNumber d:type=""cDeltaString""></UniqueNumber>
			<GUID d:type=""cDeltaString""></GUID>
			<Followers>
			</Followers>
			<TotalMass d:type=""sFloat32"" d:alt_encoding=""0000000083904C40"" d:precision=""string"">57.129</TotalMass>
			<Speed d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</Speed>
			<Velocity>
				<cHcRVector4>
					<Element>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
						<e d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</e>
					</Element>
				</cHcRVector4>
			</Velocity>
			<InTunnel d:type=""bool"">0</InTunnel>
			<CoalLevel d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</CoalLevel>
			<WaterLevel d:type=""sFloat32"" d:alt_encoding=""0000000000000000"" d:precision=""string"">0</WaterLevel>
		</cTender>
		<cAnimObjectRender d:id="""">
			<DetailLevel d:type=""sInt32"">10</DetailLevel>
			<Global d:type=""bool"">0</Global>
		</cAnimObjectRender>
		<cCargoComponent d:id="""">
			<IsPreLoaded d:type=""cDeltaString"">eFalse</IsPreLoaded>
			<InitialLevel>
			</InitialLevel>
		</cCargoComponent>
		<cEntityContainer d:id="""">
			<StaticChildrenMatrix>
			</StaticChildrenMatrix>
		</cEntityContainer>
		<cScriptComponent d:id="""">
			<DebugDisplay d:type=""bool"">1</DebugDisplay>
			<StateName d:type=""cDeltaString""></StateName>
		</cScriptComponent>
	</Component>
	<BlueprintID>
		<iBlueprintLibrary-cAbsoluteBlueprintID>
			<BlueprintSetID>
				<iBlueprintLibrary-cBlueprintSetID>
					<Provider d:type=""cDeltaString""></Provider>
					<Product d:type=""cDeltaString""></Product>
				</iBlueprintLibrary-cBlueprintSetID>
			</BlueprintSetID>
			<BlueprintID d:type=""cDeltaString""></BlueprintID>
		</iBlueprintLibrary-cAbsoluteBlueprintID>
	</BlueprintID>
	<ReskinBlueprintID>
		<iBlueprintLibrary-cAbsoluteBlueprintID>
			<BlueprintSetID>
				<iBlueprintLibrary-cBlueprintSetID>
					<Provider d:type=""cDeltaString""></Provider>
					<Product d:type=""cDeltaString""></Product>
				</iBlueprintLibrary-cBlueprintSetID>
			</BlueprintSetID>
			<BlueprintID d:type=""cDeltaString""></BlueprintID>
		</iBlueprintLibrary-cAbsoluteBlueprintID>
	</ReskinBlueprintID>
	<Name d:type=""cDeltaString""></Name>
	<EntityID>
	</EntityID>
</cOwnedEntity>";

		public static string GetXml(VehicleType type)
        {
			switch(type)
            {
				case VehicleType.Engine:
					return EngineXml;
				case VehicleType.Wagon:
					return WagonXml;
				case VehicleType.Tender:
					return TenderXml;
				default:
					throw new Exception("Unknown vehicle type.");
            }
        }
	}
}
