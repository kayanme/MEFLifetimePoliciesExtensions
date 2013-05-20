This is simple MEF extension for scoping part in non-standart, but useful way (i.e. per-thread, per-transaction etc.) along with base classes to implement own realisations.

To use this extensions, do the following:
1. Add the whole assembly to the container catalog.
2. Use ThreadPolicy &lt;TYourType&gt; (for example) in constructor or property injection to compose part of TYourType in desired scope.
   Mention, that the part must be non-shared (just to have sense for scoping).

The lifetime of a part is controlled only by affinity and your usage - when the thread and the scope die and all references to your object are gone, 
it is free for GC. Of course, no explicit disposing is done (except you want to call it yourself, but it is bad thing in scoping).

Example:

[Export]
class Outer
{

  [Import]
  ThreadPolicy &lt;Inner&gt; _perThreadObject;
  ...
}

or 

[Export]
class Outer
{

  Inner __perThreadObject;

  [ImportingConstructor]
  Outer([Import]ThreadPolicy &lt;Inner&gt; perThreadObject)
  {
      _perThreadObject = perThreadObject; //yes,there is implicit conversion
  }
  
}