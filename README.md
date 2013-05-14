C8POC
=====

Chip8 Proof of Concept

Created by Albert Rodríguez Franco 2013

TODO: 

-Revision of not implemented opcodes

-Review how multithreading behaves in performance

-Separation of responsabilities

	- Plugin manager is more like a service than a static instance, put it inside the engine.
	- Clean up code related to the system configuration
	- General arquitechture picture revision

-Creation of a disassembler to view what's happening with the machine in real time

-Monitorize the real speed of the emulator (e.g. show FPS in the Graphics Plugin)

-Make the engine also an interface to have full control of injection

-Unit testing (something left apart that should take relevance)
