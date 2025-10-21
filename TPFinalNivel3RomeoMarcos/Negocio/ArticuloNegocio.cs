using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using Datos;

namespace Negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listaArticulo;
        AccesoDatos datos = new AccesoDatos();
       
        private List<Articulo> CargarListaArticulo(AccesoDatos datos)
        {
            listaArticulo = new List<Articulo>();
            try
            {
                while (datos.Lector.Read())
                {
                    Articulo articulo = new Articulo();
                    articulo.Id = datos.Lector.GetInt32(0);
                    articulo.Codigo = (string)datos.Lector["Codigo"];
                    articulo.Nombre = (string)datos.Lector["Nombre"];
                    articulo.Descripcion = (string)datos.Lector["Descripcion"];
                    articulo.Marca = (string)datos.Lector["Marca"];
                    articulo.Categoria = (string)datos.Lector["Categoria"];
                    articulo.ImagenUrl = (string)datos.Lector["ImagenUrl"];
                    articulo.Precio = (decimal)(datos.Lector["Precio"]);

                    listaArticulo.Add(articulo);
                }
                return listaArticulo;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<Articulo> ListarArticulos()
        {

            try
            {
                datos.SetearConsulta("select a.Id, a.Codigo, a.Nombre, a.Descripcion, ISNULL(m.Descripcion, 'Sin Marca') Marca,ISNULL(c.Descripcion, 'Sin Categoría') Categoria, a.ImagenUrl, a.Precio from Articulos a left join MARCAS m on a.IdMarca = m.Id left join CATEGORIAS c on a.IdCategoria = c.Id");
                datos.Leer();

                return CargarListaArticulo(datos);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public Dictionary<int, string> ListaMarca()
        {
            try
            {
                Dictionary<int, string> listaMarcas = new Dictionary<int, string>();
                datos.SetearConsulta("select Id, descripcion from Marcas");
                datos.Leer();
                listaMarcas.Add(0,"");
                while (datos.Lector.Read())
                {
                    int id = datos.Lector.GetInt32(0);
                    string descripcion = (string)datos.Lector["descripcion"];
                    listaMarcas.Add(id,descripcion);
                }
                datos.CerrarConexion();

                return listaMarcas;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Dictionary<int, string> ListaCategoria()
        {
            try
            {
                Dictionary<int,string> listaCategoria = new Dictionary<int, string>();
                datos.SetearConsulta("select id, descripcion from Categorias");
                datos.Leer();
                listaCategoria.Add(0, "");
                while (datos.Lector.Read())
                {
                    int id = datos.Lector.GetInt32(0);
                    string descripcion = (string)datos.Lector["descripcion"];
                    listaCategoria.Add(id,descripcion);
                }
                datos.CerrarConexion();

                return listaCategoria;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        
        public List<Articulo> BusquedaAvanzada(string campo, string criterio, string filtro)
        {
            string consulta = "select a.Id, a.Codigo, a.Nombre, a.Descripcion, m.Descripcion Marca, c.Descripcion Categoria, a.ImagenUrl, a.Precio from Articulos a inner join MARCAS m on a.IdMarca = m.Id inner join CATEGORIAS c on a.IdCategoria = c.Id where ";
            try
            {
                if (campo == "Id")
                {
                    
                    switch (criterio)
                    {
                        case "Igual a":
                            consulta += "a." + campo + " = " + filtro;
                            break;
                        case "Mayor a":
                            consulta += "a." + campo + " > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "a." + campo + " < " + filtro;
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Contenga":
                            consulta += "a." + campo + " like '%" + filtro + "%' ";
                            break;
                        case "Empice con":
                            consulta += "a." + campo + " like '" + filtro + "%' ";

                            break;
                        case "Termine con":
                            consulta += "a." + campo + " like '%" + filtro +"'";

                            break;
                    }
                }
                else if (campo == "Marca")
                {
                    switch (criterio)
                    {
                        case "Contenga":
                            consulta += "m.Descripcion like '%" + filtro + "%' ";
                            break;
                        case "Comience con":
                            consulta += "m.Descripcion like '" + filtro + "%' ";

                            break;
                        case "Termine con":
                            consulta += "m.Descripcion like '%" + filtro + "'" ;

                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Contenga":
                            consulta += "c.Descripcion like '%" + filtro + "%' ";
                            break;
                        case "Comience con":
                            consulta += "c.Descripcion like '" + filtro + "%' ";

                            break;
                        case "Termine con":
                            consulta += "c.Descripcion like '%" + filtro + "'" ;

                            break;
                    }
                }
                datos.SetearConsulta(consulta);
                datos.Leer();
                CargarListaArticulo(datos);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return listaArticulo;
        }

        public void AgregarArticulo(Articulo articulo, int IdMarca, int IdCategoria)
        {
            try
            {
                datos.SetearConsulta($"insert into ARTICULOS values('{articulo.Codigo}','{articulo.Nombre}','{articulo.Descripcion}',{IdMarca},{IdCategoria},'{articulo.ImagenUrl}',{articulo.Precio})");
                datos.Escribir();
                datos.CerrarConexion();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void AgregarMarcaCategoria(string tabla, string nombre)
        {
            try
            {
                datos.SetearConsulta($"insert into {tabla} values('{nombre}')");
                datos.Escribir();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {

                datos.CerrarConexion();
            }
        }

        public void ModificarArticulo(Articulo articulo, int IdMarca, int IdCategoria)
        {
            try
            {
                datos.SetearConsulta($"update Articulos set Nombre= '{articulo.Nombre}', Codigo= '{articulo.Codigo}', Descripcion= '{articulo.Descripcion}', ImagenUrl= '{articulo.ImagenUrl}', IdMarca= {IdMarca}, IdCategoria= {IdCategoria}, Precio= @precio where id = {articulo.Id}");

                datos.SetearParametros("@precio", articulo.Precio);

                datos.Escribir();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();

            }

        }

        public void ModificarMarcaCategoria(int id, string nombreNuevo, string bandera)
        {
            try
            {
                datos.SetearConsulta($"update {bandera} set descripcion = '{nombreNuevo}' where Id = {id}");

                datos.Escribir();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }

        }

        public void Borrar(int id, string tabla)
        {
            try
            {
                datos.SetearConsulta($"delete from {tabla} where id = {id}");
                datos.Escribir();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }


        }
    }
}
