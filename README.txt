Some notes...

* This is no official alpha release! Feel free to test it though.

* Envers needs a "special" NH Core to run. Not even NH trunk works, only the NH bits Envers reference in this project.

* Most of Envers functionality is ported, you can read manual here... http://docs.jboss.org/envers/docs/index.html. There's a short cut for configuration though - simply call IntegrateWithEnvers on your NH Configuration object.

* Hopefully an alpha release will be available when NH 3.01 is released.

* Right now you can only configure what to audit by using attributes (like Envers). Hopefully there will be alternatives soon.


