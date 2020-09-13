#!/bin/bash

main_branch=main
dev_branch=develop

branch=$(git rev-parse --abbrev-ref HEAD)
latest=$(git describe --abbrev=0 --tags)

semver_parts=(${latest//./ })
major=${semver_parts[0]}
minor=${semver_parts[1]}
patch=${semver_parts[2]}

increment=$(git rev-list ${latest}..HEAD --ancestry-path ${latest} --count)

if [[ increment -eq 0 ]]
then
  version=${latest}
else
  case $branch in
    $main_branch)
      version=${major}.$((minor+1)).0
      ;;
    $dev_branch)
      version=${major}.${minor}.$((patch+1))
      ;;
    "feature/*")
      version=${latest}-${branch}-${increment}
      ;;
    *)
      >&2 echo "unsupported branch type"
      exit 1
      ;;
  esac
fi

echo ${version}
exit 0
