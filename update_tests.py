#!/usr/bin/env python3
"""
Script to update remaining TST DTO test instantiations and property accesses
"""

import re
import sys

def update_user_request_dto(content):
    """Update TstUserRequestDto instantiations to use BaseUserDto"""
    # Pattern for old style: new TstUserRequestDto("id", "username", "email", "ext1", "ext2")
    # Handle both single-line and multi-line patterns
    pattern = r'new TstUserRequestDto\(\s*"([^"]+)",\s*"([^"]+)",\s*"([^"]+)",\s*"([^"]+)",\s*"([^"]+)"\s*\)'
    replacement = r'new TstUserRequestDto(new BaseUserDto("\1", "\2", "\3"), "\4", "\5")'
    content = re.sub(pattern, replacement, content, flags=re.MULTILINE | re.DOTALL)
    
    return content

def update_user_property_access(content):
    """Update property accesses to use BaseUser"""
    replacements = [
        (r'returnedUser\.Id', 'returnedUser.BaseUser.Id'),
        (r'returnedUser\.UserName', 'returnedUser.BaseUser.UserName'),
        (r'returnedUser\.Email', 'returnedUser.BaseUser.Email'),
        (r'returnedUsers\.First\(\)\.Id', 'returnedUsers.First().BaseUser.Id'),
        (r'returnedUsers\.First\(\)\.UserName', 'returnedUsers.First().BaseUser.UserName'),
        (r'returnedUsers\.First\(\)\.Email', 'returnedUsers.First().BaseUser.Email'),
    ]
    
    for pattern, replacement in replacements:
        content = re.sub(pattern, replacement, content)
    
    return content

def main():
    if len(sys.argv) < 2:
        print("Usage: python update_tests.py <file_path>")
        sys.exit(1)
    
    file_path = sys.argv[1]
    
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Apply updates
    content = update_user_request_dto(content)
    content = update_user_property_access(content)
    
    with open(file_path, 'w') as f:
        f.write(content)
    
    print(f"Updated {file_path}")

if __name__ == '__main__':
    main()

