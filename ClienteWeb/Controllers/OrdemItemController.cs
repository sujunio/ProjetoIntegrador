﻿using Modelo;
using Servico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ClienteWeb.Controllers
{
    [Authorize]
    public class OrdemItemController : Controller
    {
        private Contexto db = new Contexto();

        //lista itens da Ordem
        public ActionResult ListarItens(int id)
        {
            var lista = db.OrdemItens
                .AsNoTracking()
                .Include(n => n.Ordem)
                .Include(n => n.ServicoP)
                .Where(m => m.Ordem.Id == id)
                .ToList();

            ViewBag.OrdemId = id;
            ViewBag.ServicoId = new SelectList(db.Servicos, "Id", "Descricao");
            //Em Cada iten eu calculo seu valor
            foreach (var item in lista)
            {
                item.Total = item.Valor * (item.Quantidade);
            }

            //se a lista tiver mais que 0 itens
            if (lista.Count() > 0)
                ViewBag.TotalItens = lista.Sum(n => n.Valor * n.Quantidade);//então calculo total de todos o itens
            else
                ViewBag.TotalItens = 0;

            return PartialView(lista);
        }
        
        //salvar itens na ordem
        public ActionResult SalvarItens(
            int quantidade, 
            int servicoId, 
            decimal valor, 
            int ordemId)
        {

            var item = new OrdemItem()
            {
                //OrdemId = ordemId,
                ServicoId = servicoId,
                Quantidade = quantidade,
                Valor = valor,
                Ordem = db.Ordens.Find(ordemId)
            };

            db.OrdemItens.Add(item);
            db.SaveChanges();

            return Json(new { Resultado = item.Id }, JsonRequestBehavior.AllowGet);
        }

        //excluir item da ordem
        [HttpPost]
        public ActionResult Excluir(int id)
        {
            var result = false;
            var item = db.OrdemItens.Find(id);

            if (item != null)
            {
                db.OrdemItens.Remove(item);
                db.SaveChanges();
                result = true;
            }

            return Json(new { Resultado = result }, JsonRequestBehavior.AllowGet);
        }

    }
}