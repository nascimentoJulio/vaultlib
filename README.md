### Sobre o vault:

#### O que é:
O vault, como o nome sugere, é um cofre para armazenar senhas e chaves de criptografia de forma segura. Com o vault podemos fazer controles de autorização, obter logs e fazer a rotação das nossas credenciais de maneira simples.

#### Como funciona:
O vault guarda suas credenciais de uma maneira _"path-based"_, pois as credenciais são acessadas e salvas através de uma rota da escolha do usuario. Existem duas rotas que vem por default: /secrets e /cubbyhole. Podemos manipular essas e outras rotas de 4 maneiras: via CLI, via UI, via biblioteca (caso esteja disponivel para a sua linguagem) e via uma API rest que é disponibilizada.

#### Como subir um ambiente:

Existem algumas formas de configurar um ambiente vault, porém aqui focaremos em subir uma instancia no `docker` (pressupondo que você já o tenha instalado). Primeiro de tudo, usando um editor de texto da sua preferência vamos criar um arquivo compose da seguinte forma:
![[Pasted image 20230102200521.png]]

~~~yaml
version: '3.8'

services:
  vault:
    image: vault
    user: root
    cap_add:
      - "IPC_LOCK"
    ports:
      - 8200:8200
    environment:
      VAULT_DEV_ROOT_TOKEN_ID: mytoken
      VAULT_DEV_LISTEN_ADDRESS: 0.0.0.0:1234
    volumes:
      - ./vault/auditoria:/vault/logs
      - ./volumes/config:/vault/config
    command: vault server -dev -config=/vault/config/vault.json
~~~

Dessa forma subimos um container em modo de desenvolvimento para que não seja necessário fazer algumas configurações extras (Não deve ser usado dessa forma em ambiente produtivo). Precisamos ainda criar esse arquivo de configurações no diretorio:
`/vault/config/vault.json`. O arquivo ficará da seguinte forma:
~~~json
{
  "storage": { "file": { "path": "/vault/file" } },
  "listener": [{ "tcp": { "address": "0.0.0.0:8200", "tls_disable": true } }],
  "default_lease_ttl": "168h",
  "max_lease_ttl": "720h",
  "ui": true
}
~~~
pronto, agora é só rodar nosso compose com o comando: `docker-compose up` e correr pro abraço.
![[Pasted image 20230102202307.png]]
Note que o `mytoken` foi definido no docker e pode conter o valor que você quiser.

Pronto, agora que temos que nosso sistema no ar, podemos criar nossa lib para obter credenciais.

#### Criando a Lib
Vamos criar uma biblioteca para gerenciar essas chaves e para isso vamos em:
![[Pasted image 20230103092138.png]]
![[Pasted image 20230103092350.png]]
![[Pasted image 20230103092355.png]]

![[Pasted image 20230103092359.png]]
pronto, agora que temos nossa biblioteca, adicionaremos a biblioteca sugerida pela hashicorp:
![[Pasted image 20230103092606.png]]

pronto, agora só criarmos os métodos e para isso vamos criar uma interface da seguinte forma:

~~~C#
namespace VautlLib
{
    public interface IVaultHandler
    {
        Task<object> GetCredentials(string path);
    }
}
~~~

>**Obs**: caso queira mapear o objeto retornado pela lib fique a vontade, aqui foi usado `object` para facilitar. Outra observação é que aqui só será obtido as credenciais, mas fique a vontade para implementar o crud todo.

Agora só implementar a interface do seguinte modo:

~~~C#
public class VaultHandler : IVaultHandler
{
	private static IVaultClient _vaultCLient;
	
	public VaultHandler(HttpClient client)
    {
        _vaultClient = InstanceVaultClient();
        _client=client;
    }
	public async Task<object> GetCredentialsFromVaultLib(string path)
    {
	    return await _vaultCLient().V1
					   .Secrets
					   .KeyValue
					   .V2
				       .ReadSecretAsync(path: path, mountPoint:        
										$"/secret/data/{path}");
    }

    private IVaultClient InstanceVaultClient()
    {
        if (_vaultClient == null)
        {
          IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken: 
														        "mytoken");
	      VaultClientSettings vaultClientSettings = new 
				      VaultClientSettings("http://localhost:8200", authMethod);
                _vaultClient = new VaultClient(vaultClientSettings);
        }
	    return _vaultClient;
    }
}
~~~
