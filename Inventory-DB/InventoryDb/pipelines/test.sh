#!/bin/bash
# Diagnose and fix PATH issues

# View current PATH
echo "Current PATH:"
echo "$PATH" | tr ':' '\n'

# Check if a directory is in PATH
check_in_path() {
    local dir="$1"
    if [[ ":$PATH:" == *":$dir:"* ]]; then
        echo "$dir is in PATH"
        return 0
    else
        echo "$dir is NOT in PATH"
        return 1
    fi
}

check_in_path "/usr/local/bin"
check_in_path "$HOME/.local/bin"

# Find where a command is located
find_command() {
    local cmd="$1"

    # Check if it exists anywhere on the system
    local locations=$(find /usr /opt "$HOME" -name "$cmd" -type f 2>/dev/null)

    if [[ -n "$locations" ]]; then
        echo "Found $cmd at:"
        echo "$locations"
    else
        echo "$cmd not found on system"
    fi
}

# Add directory to PATH for current session
#export PATH="$PATH:/new/directory"

# Add directory to PATH permanently
# Add to ~/.bashrc or ~/.bash_profile:
# export PATH="$PATH:$HOME/.local/bin"