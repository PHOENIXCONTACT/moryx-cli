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
