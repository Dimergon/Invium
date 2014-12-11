PinCushion, a password manager in C#
Copyright (c) 2013, 2014 Armin Altorffer

PinCushion is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PinCushion is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with PinCushion.  If not, see <http://www.gnu.org/licenses/>.

Features:

- Create an infinite number of profiles, each with an infinite
  number of services, each of which with an infinite number of
  accounts.

- Cross-platform, coded in C# and fully functional in both .NET
  and Mono.

- Data is encrypted both on disk and in memory with AES 256-bit
  in conjunction with SHA512 hashes although in-memory encryption
  is only enabled on Windows as ram scrapers are unlikely on Linux
  and other *nix platforms supported by Mono.

- Targeting .NET 4.5 to avoid compatibility with depricated and
  unsupported operating systems such as Windows XP.

- Very intuitive, easy to use interface.
