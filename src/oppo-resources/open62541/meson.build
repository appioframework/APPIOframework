
project('simple-meson-example', 'c',
        version : '1.0',
        default_options : [ 'warning_level=3', 'buildtype=debugoptimized', 'c_std=c11' ]
)

app_sources  = [ files('src/main.c', 'src/open62541.c'),
]

app_inc      = [ include_directories('src')
]

app_deps     = [ 
]

app_exe      = executable('app',
                          app_sources,
                          include_directories: app_inc,
                          dependencies:        app_deps)
