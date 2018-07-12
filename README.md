STUN
====
Ported by ASH KATCHAP from [hf/stun](https://github.com/hf/stun)
Needs the libraries: 
[HashUtils](https://github.com/forestrf/HashUtils)
[BBuffers](https://github.com/forestrf/BBuffer), 
[NoGcSockets](https://github.com/forestrf/No-gc-sockets), 

This is a small, pure-C# library that implements the STUN message format with a message builder, a message parser, and its attributes.

This port is optimized to prevent generating garbage by using structs and singletons and reusing the byte array that contains the data as much as possible.

## License

Copyright &copy; 2018 Andrés Leone

This code is licensed under the permissive MIT X11 license. For the full text
see `LICENSE.txt`.
