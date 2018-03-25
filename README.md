# Narchive

A command-line app for creating and extracting NARC archives.

## Usage
### Create
```
narchive create <output> <input> [options]
```

#### Arguments

**output**

The name of the NARC archive to create.

**input**

The folder containing the files and folders to add to the NARC archive.

#### Options

**-nf** or **--nofilenames**

Specifies the entries in the NARC archive will not have filenames.

**-?** or **-h** or **--help**

Shows help information for the create command.

### Extract
```
narchive extract <input> [options]
```

#### Arguments

**input**

The name of the NARC archive to extract.

#### Options

**-o &lt;output&gt;** or **--output &lt;output&gt;**

The name of the folder to extract the NARC archive to. If omitted, the files will be extracted to the current folder.

**-nf** or **--nofilenames**

Ignores entry filenames and extracts using its index.

**-?** or **-h** or **--help**

Shows help information for the extract command.

## Batch files

This app comes with two batch files, create.cmd and extract.cmd, to easily create and extract NARC archives.

### create.cmd
To use this batch file, drag the folder containing the files and folders you want to add to the NARC archive. The batch file will will ask you to name the NARC archive when creating it, and if the entries should have filenames.

### extract.cmd
To use this batch file, drag the NARC archive you want to extract. The batch file will ask you for the name of the folder to extract the files to. If left blank, the files will be extracted to the current folder.

## License
Narchive is licensed under the [MIT license](LICENSE.md).