# CHANGELOG 1.5.1.0 (25-02-2023)
* Fix crash when listing scenarios on routes with no `Scenarios` dir
* Handle malformed `ScenarioProperties.xml`

# CHANGELOG 1.5.0.0 (22-02-2023)
* Vehicule number now preserved by default if involved in couple/uncouple operations
* Scenarios inside .ap files can now be viewed and edited
* Filters now search on input words individually
* Option to hide played scenarios added
* Fix Assets directory tree scrolling to previously selected item
* Scenario completion status now shown as `?` when SDBCache.bin is erroneous
* Italian language added
* Case now ignored when matching rolling stock paths

# CHANGELOG 1.4.0.0 (25-01-2023)
* Length of missing and available vehicles displayed
* Filters added for routes and scenarios lists
* Scenarios completion status now displayed
* Scenario author now displayed
* French language added
* Free roam and Quick drive scenarios now identified as such in the `Player train` field

# CHANGELOG 1.3.0.0 (21-12-2022)
* Option to apply rules to all stocks added
* The [LoSw] suffix is now a setting and can be modified or disabled
* Scenarios list now shows scenario informations (time of day, season, duration)
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
