using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkXR
{
    public class LoadTexure : ApiBase<Texture2D>
    {

        public Texture Texture { get; private set; } = null;

        public IEnumerator Send(string path)
        {
            HttpQueryParam param = new HttpQueryParam();
            yield return GetTexture(path, param.ToString());
        }

        protected override void OnTextureSuccess(Texture response)
        {
            base.OnTextureSuccess(response);
            //Debug.Log("============ applist serrch result:" + response.code + "; list:" + response.result.Count);
            Texture = response;
        }

        protected override void OnFailed(string error)
        {
            base.OnFailed(error);
            Texture = null;
        }
    }
}

