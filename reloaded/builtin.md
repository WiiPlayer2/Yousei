# Connectors

## log
### Actions
#### write
```cs
{
    IParameter Level;
    IParameter Tag;
    IParameter Message;
}
```

## control
### Actions
#### if
```cs
{
    IParameter If;
    List<BlockConfig> Then;
    List<BlockConfig> Else;
}
```
Executes **Then** if **If** is true, otherwise **Else**

#### foreach
```cs
{
    IParameter Collection;
    List<BlockConfig> Actions;
}
```
Executes all **Actions** on all elements in **Collection**

#### switch

#### while
```cs
{
    IParameter Condition;
    List<BlockConfig> Actions;
}
```
Execute all **Actions** while **Condition** is true

## flow
### Actions
#### invoke
```cs
{
    IParameter Name;
}
```
Invokes flow **Name** synchronously.

#### start
```cs
{
    IParameter Name;
}
```
Starts flow **Name** asynchronously (context will be cloned).

## data
### Actions
#### set
```cs
{
    IParameter Path;
    IParameter Value;
}
```
Sets **Path** in context to be **Value**.

## internal
### Triggers
#### onvalue
```cs
{
    string Topic;
}
```
Waits for a value being send by `sendvalue` with topic **Topic**.

#### onexception
```cs
{}
```
Waits for an exception raised internally.

### Actions
#### sendvalue
```cs
{
    IParameter Topic;
    IParameter Value;
}
```
Triggers an `onvalue` event with value **Value** on topic **Topic**.
