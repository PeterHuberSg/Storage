# Introduction
This document describes the design of Storage and how to use it.

# Design principals

## Supported relationships

Storage supports the following relationships between 2 objects:  
**"one to conditional"** (1:c)  
**"conditional to conditional"** (c:c)  
**"one to many conditional"** (1:mc)  
**"conditional to many conditional"** (c:mc)

**"one to one"** (1:1) should not be needed, because in praxis this means they form together only
1 object.

**"one to many"** (1:m) is not possible, because both objects must be created precisely at the 
same time, one cannot exist without the other. But Storage allows only the creation 
(more precisely: storage, retrieval) of own object at a time. When an instance get stored, 
all its properties must have legal values, i.e. cannot be null, unless it is nullable. For 
the same reason, 1:1 is not allowed.

**"many x to many x"** (m:m), (m:mc), (mc:mc): Like with relational data tables, 
many to many is not supported. Instead a third child class must be designed 
additionally to the two parents, which need the mc:mc relationship:

```
Parent1 1 : mc   
??       ??    Child  
Parent2 1 : mc 
``` 

## Parents first
The parent objects must be created before the child or the children linking to it. Storage
stores all instances of one class in 1 file. On application startup, the file gets read 
completely and all its instances created immediately, before the next file with the next 
class gets read. It must be possible to create any parent without any children, meaning
child is always conditional.

## Children link to parents
The child defines to which parent(s) it belongs. For each parent it has one property. When 
a child property linking to a parent gets stored, the child gets added to the children 
collection in its parent. When a stored instance changes its parent to another parent, the 
child gets removed from the first and added to the children collection of the second 
parent.

The parents get written to a file without any children information. When the parent gets 
read during startup, the parents have no children. The children get added to the parent 
when the children files gets read.

When using Storage, the parents always show their children, when they have any. "Children
link to parents" is only relevant in the sense that adding or removing a child from its
parent can only be achieved by changing the value of the child property linking to the 
parent. There is no method supporting removing the child directly form the children 
collection in the parent.

## Relationship notation
C:P
1:c
c:c
1:mc
c:mc

C is the child, can only be 1 or c (in this document always first)  
P is the parent, can only be c or mc (in this document always second)

1: single  
c: conditional  
m: multiple

## Nullability indicates conditionality
Storage supports nullable reference types. A property for a conditional relationship is
nullable, unless for many relationships, where a collection (List, Dictionary, SortedList) 
is used. The collection property is not nullable, but the collection can be empty (= no 
child).

The parent must be able to exist without children, therefore the property must be nullable 
for 1:c and c:c or a collection for 1:mc or c:mc.

A child property can link to only 1 parent at a time. It's type is the reference type of 
its parent. The property is nullable for c:c or c:mc, not nullable for 1:c or 1:mc.

## Relationship examples
1:c
```c#
public class Parent {
  public Child? Child;
}

public class Child {
  public Parent Parent;
}
```
c:c
```c#
public class Parent {
  public Child? Child;
}

public class Child {
  public Parent? Parent;
}
```
1:mc
```c#
public class Parent {
  public List<Child> Children;
}

public class Child {
  public Parent Parent;
}
```
c:mc
```c#
public class Parent {
  public List<Child> Children;
}

public class Child {
  public Parent? Parent;
}
```
mc:mc
```c#
public class Parent1 {
  public List<Child> Children;
}

public class Parent2 {
  public List<Child> Children;
}

public class Child {
  public Parent1 Parent1;
  public Parent2 Parent2;
}
```

## Data Model

The Data Model defines all classes and their relationships among them as shown in 
*Relationship examples*. The StorageClassGenerator reads the Data Model and creates the
Data Context and for each class in the Data Model a new class supporting the creation, 
storing, updating and deleting (removal) of instances.

## Generated Classes
As with any other C# class, the constructor **new()** is called to create a new instance. The
constructor has a *isStoring* parameter, which indicates if the instance should get written
immediately to a file. Otherwise, the instance can get written by calling its **Store()** 
method. If a child property has a parent value, the child gets added to the children 
collection of the parent.

StorageClassGenerator adds to each class a **Key** property. Its value is smaller 0 until the
instance gets stored, at which time the key gets a unique value.

None of the properties can be changed directly, they must be changed by calling
**Update()**. This allows changing and writing the new content to the file of several 
properties at the same time. For performance reason, it is important that the writing 
does not get executed for every single property that changes. 

If an instance is no longer needed, **Remove()** can be called. Remove unlinks the 
instance from any other instances and deletes it from the file.
 

## Data Context (DC)
The DC gives access to all data. For each class in the Data Model, a collection gets 
created. A particular instance can get retrieved through its Key value from that 
collection as soon as it is stored. Enumeration supports accessing all instances.

At startup, the DC reads all files and fills the collections with instances. When the
program runs, data changes get automatically written to the files, although with a
small time delay to write many changes at the same time, which improves performance. At
application shutdown, the DC guarantees that all data is written (flushed) to the files.

DC has a static property Data, which holds all the collections. The application can acces
and data through DC.Data.CollectionName.

DC supports transactions:

```c#
DC.Data.StartTransaction();
var parent = new Parent(..., isStoring: true);
var child = new Child(Parent, ..., isStoring: false);
child.Store();
child.Update(...);
if (ok){
  DC.Data.CommitTransaction();
}else{
  DC.Data.RollbackTransaction();
}
```

Any data change caused by new(), Store(), Update() or Remove() gets directly written to a
file, even before DC.Data.CommitTransaction() is called. RollbackTransaction() removes
 whatever was written since StartTransaction() from the files.