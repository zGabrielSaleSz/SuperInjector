## Super Injector

This is not a simple injector, this is actually a SUPER INJECTOR, but simple.

First of all, it's interesting the power of a dependency injector... But they overkill in features, and like Uncle Ben said: "With great power comes great responsibility"

With too many responsibilities is hard to make sure all the context is healthy, suddenly there is an attempt to make sure you're not manipulating more classes after some of them were built, bringing limitations.

Well, In the past years, I've been working with SimpleInjector, Windsor, and Microsoft's injector, and all of them work fine, Microsoft's injector usually works better for me related to verbosity and complexity of use. So why shouldn't I build an injector that works and looks simple like I want?

Furthermore, reinvent the wheel is the best way to understand the difficulty that others have passed until their final solution. It's the way I like to learn new things.

## Target
The idea of **SuperInjector** is to be simple, and useful, with minimal limitations, so dependency injections could be simple and flexible.

On the other hand, it may help you to understand how Dependency Injector works under the hood if you're currently studying dependency injection.

This injector is not supposed to work with "Scoped" as Microsoft does (web request scope) because currently, you don't need something like that, The concept of scope can be relative and I haven't figured out the best approach to make it efficient.

If you're working on a new web project your best choice will be Microsoft's injector since it is designed for it, and is present in the "hello world" project.