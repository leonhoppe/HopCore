fx_version 'cerulean'
games { 'gta5' }

author 'Leon hoppe <leon@ladenbau-hoppe.de>'
description 'FiveM Framework in C#'
version '1.0.0'

files {
    'Newtonsoft.Json.dll',
    'HopCore.Shared.dll',
    'HopCore.Client.dll',
    'HopCore.Server.dll'
}

client_scripts {
    'HopCore.Client.Core.net.dll'
}

server_scripts {
    'HopCore.Server.Core.net.dll'
}
