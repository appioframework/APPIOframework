
project('simple-meson-example', 'c',
        version : '1.0',
        default_options : [ 'warning_level=3', 'buildtype=debugoptimized', 'c_std=c11' ]
)

client_app_sources  = [ files('src/client/main.c', 'src/client/open62541.c'),
]

client_app_inc      = [ include_directories('src/client')
]

client_app_deps     = [ 
]

client_app_exe      = executable('client-app',
                          client_app_sources,
                          include_directories: client_app_inc,
                          dependencies:        client_app_deps)

server_app_sources  = [ files('src/server/main.c', 'src/server/open62541.c'),
]

server_app_inc      = [ include_directories('src/server')
]

server_app_deps     = [ 
]

server_app_exe      = executable('server-app',
                          server_app_sources,
                          include_directories: server_app_inc,
                          dependencies:        server_app_deps)
