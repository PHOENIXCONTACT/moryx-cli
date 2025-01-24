     __  __    ___    ____   __   __ __  __       ____   _       ___        __  
    |  \/  |  / _ \  |  _ \  \ \ / / \ \/ /      / ___| | |     |_ _|    _  \ \  
    | |\/| | | | | | | |_) |  \ V /   \  /      | |     | |      | |    (_)  | |  
    | |  | | | |_| | |  _ <    | |    /  \   _  | |___  | |___   | |     _   | |  
    |_|  |_|  \___/  |_| \_\   |_|   /_/\_\ (_)  \____| |_____| |___|   (_)  | |  
                                                                            /_/  

# MORYX.CLI

## `new`

Creates a new MORYX solution, optionally with some products and steps already. 

### Usage
    
    moryx new <NAME> [--products <PRODUCTS>] [--steps <STEPS>] [--no-git-init]


## `add product`

Adds a product 'name'

### Usage

    moryx add product <NAME>


## `add state`

Adds a 'state machine' to a specified context. It adds a folder `States` next
to the file that contains the context including a `StateBase` and `State` files
for each provided state. The context will be updated to implement `IStateContext`.

Adding further states later is possible and would add the new states and update
the existing `StateBase` accordingly.

_Note_: Adding states to the `States` folder may lead conflicts if you try to 
create states for several contexts that live inside the same path. We recommend
to move those 'to be contexts' to separate directories or libraries before
adding state machines in order to maintain a clean project structure.


### Usage

    moryx add states <CONTEXT> --states <Comma separated list of states>

Example

    moryx add states AssemblingCell --states "Initialing, Running, Testing"


 
## `add step`

Adds a production step 'name', that is a cell with a task, aktivity, capabilites
and so on.

### Usage

    moryx add step <NAME> [--without-cell]


## `exec` 

Exec is meant to execute 'command' against a running MORYX instance. 
This is rather a workaround. 

### Usage

    moryx exec <COMMAND> [--endpoint]

### Examples

    moryx exec create-dbs
