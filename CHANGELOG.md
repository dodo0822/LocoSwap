# CHANGELOG 1.3.0.0 (21-12-2022)
* Option to apply rules to all stocks added
* The [LoSw] suffix is now a setting and can be modified or disabled
* Scenarios list now show scenario informations (time of day, season, duration)
* Replacement rules and scanned vehicles can now be filtered with a text field
* App now has an icon :)
* Various quality of life improvements

# CHANGELOG 1.2.0.0 (23-11-2022)
* Automatically create replacement rules every time you hit Replace all (can be disabled with a ticking box)
* Vehicles missing but with existing replacement rules will now show with yellow dots, and an Apply all rules button is provided to apply all your rules at once
* Drastically speed up the scanning of .ap files by first looking for a RailVehicles folder (and only looking in it if there is one)
* Scanned vehicles list does not empty anymore when scanning a new folder
* Fixed some portals or waypoints breaking (those containing line returns, `Tonbridge Dn Fast` on the Chatham is your typical suspect)
* Edited scenarios now appended with a [LoSw] suffix
* Fixed crash when wagon blueprint does not feature a cCargoComponent
