C8POC
=====

Chip8 Proof of Concept

Created by Albert Rodríguez Franco 2013

TODO: 

-Revision of not implemented opcodes

-Key input system (customization, emulator key configuration in app.config?)

-Bugfixing with special interest in controlling threads and timers

-Separation of responsabilities

		-Wrap emulator properties in a struct (machine state)
  
		-Isolate I/O operations (Sound, Graphics, KeyInput) in a Plugin System
